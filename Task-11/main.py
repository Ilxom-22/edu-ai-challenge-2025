#!/usr/bin/env python3
"""
Audio Transcription & Analytics Console Application

This application processes audio files through OpenAI's APIs to generate
transcriptions, summaries, and analytics.
"""

import os
import sys
import argparse
import json
import datetime
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import logging
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

try:
    import openai
    from openai import OpenAI
    import mutagen
    from mutagen.mp3 import MP3
    from mutagen.wave import WAVE
    from mutagen.mp4 import MP4
except ImportError as e:
    print(f"Missing required dependencies. Please run: pip install -r requirements.txt")
    print(f"Error: {e}")
    sys.exit(1)


class AudioAnalyzer:
    """Main class for audio transcription and analytics."""
    
    SUPPORTED_FORMATS = {'.mp3', '.wav', '.m4a', '.mp4', '.mpeg', '.mpga', '.webm'}
    MAX_FILE_SIZE = 25 * 1024 * 1024  # 25MB limit for Whisper API
    
    def __init__(self, api_key: Optional[str] = None):
        """Initialize the AudioAnalyzer with OpenAI API key."""
        self.api_key = api_key or os.getenv('OPENAI_API_KEY')
        if not self.api_key:
            raise ValueError("OpenAI API key not found. Please set OPENAI_API_KEY environment variable.")
        
        self.client = OpenAI(api_key=self.api_key)
        self.setup_logging()
        self.setup_output_directory()
    
    def setup_logging(self):
        """Setup logging configuration."""
        # Clear any existing handlers to avoid duplicates
        logging.getLogger().handlers.clear()
        
        # Set up the logger
        self.logger = logging.getLogger(__name__)
        self.logger.setLevel(logging.INFO)
        
        # Create handler if it doesn't already exist
        if not self.logger.handlers:
            handler = logging.StreamHandler(sys.stdout)
            handler.setLevel(logging.INFO)
            formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
            handler.setFormatter(formatter)
            self.logger.addHandler(handler)
            
        # Prevent propagation to root logger to avoid duplicate messages
        self.logger.propagate = False
    
    def setup_output_directory(self):
        """Create output directory if it doesn't exist."""
        self.output_dir = Path('outputs')
        try:
            self.output_dir.mkdir(exist_ok=True)
            self.logger.info(f"Output directory: {self.output_dir.absolute()}")
        except PermissionError:
            self.logger.error("Permission denied creating output directory")
            raise
    
    def validate_audio_file(self, file_path: Path) -> Tuple[bool, str]:
        """Validate audio file format and size."""
        if not file_path.exists():
            return False, f"File not found: {file_path}"
        
        if file_path.suffix.lower() not in self.SUPPORTED_FORMATS:
            return False, f"Unsupported format: {file_path.suffix}. Supported: {', '.join(self.SUPPORTED_FORMATS)}"
        
        file_size = file_path.stat().st_size
        if file_size > self.MAX_FILE_SIZE:
            return False, f"File too large: {file_size / 1024 / 1024:.1f}MB. Max: 25MB"
        
        return True, "Valid"
    
    def get_audio_duration(self, file_path: Path) -> float:
        """Get audio duration in minutes using mutagen."""
        try:
            if file_path.suffix.lower() == '.mp3':
                audio = MP3(file_path)
            elif file_path.suffix.lower() == '.wav':
                audio = WAVE(file_path)
            elif file_path.suffix.lower() in ['.m4a', '.mp4']:
                audio = MP4(file_path)
            else:
                # Fallback to mutagen.File
                audio = mutagen.File(file_path)
            
            if audio is not None and hasattr(audio, 'info') and hasattr(audio.info, 'length'):
                return audio.info.length / 60.0  # Convert to minutes
            else:
                self.logger.warning("Could not determine audio duration")
                return 0.0
        except Exception as e:
            self.logger.warning(f"Error getting audio duration: {e}")
            return 0.0
    
    def transcribe_audio(self, file_path: Path) -> str:
        """Transcribe audio using OpenAI Whisper API."""
        self.logger.info("Starting audio transcription...")
        
        try:
            with open(file_path, 'rb') as audio_file:
                response = self.client.audio.transcriptions.create(
                    model="whisper-1",
                    file=audio_file,
                    response_format="text"
                )
            
            transcription = response.strip() if response else ""
            self.logger.info(f"Transcription completed. Length: {len(transcription)} characters")
            return transcription
            
        except openai.RateLimitError:
            self.logger.error("Rate limit exceeded. Please wait and try again.")
            raise
        except openai.AuthenticationError:
            self.logger.error("Invalid API key. Please check your OpenAI API key.")
            raise
        except Exception as e:
            self.logger.error(f"Transcription failed: {e}")
            raise
    
    def generate_summary(self, transcription: str) -> str:
        """Generate summary using GPT-4.1-mini."""
        self.logger.info("Generating summary...")
        
        word_count = len(transcription.split())
        target_length = min(300, max(150, word_count // 10))
        
        prompt = f"""
        You are an expert summarization specialist. Create a comprehensive yet concise summary of the following transcription.

        ## Instructions:
        1. Target length: {target_length} words (flexible ±20%)
        2. Structure your summary with clear sections
        3. Capture the main themes, key points, and important details
        4. Preserve critical information, data, names, and conclusions
        5. Use clear, professional language
        6. Include actionable items or next steps if mentioned
        7. Maintain the logical flow of the original content

        ## Summary Format:
        **Main Topic/Purpose:** [Brief description]

        **Key Points:**
        • [Most important point 1]
        • [Most important point 2]
        • [Most important point 3]

        **Details & Context:** 
        [Elaborate on the key points with supporting details, explanations, or examples mentioned in the transcription]

        **Conclusions/Outcomes:** 
        [Any decisions made, conclusions reached, or next steps identified]

        ## Transcription to Summarize:
        {transcription}

        ## Summary:
        """
        
        try:
            response = self.client.chat.completions.create(
                model="gpt-4.1-mini",
                messages=[
                    {"role": "system", "content": "You are an expert content summarizer who creates structured, informative summaries that capture both the essence and important details of any text. Always follow the requested format and maintain accuracy."},
                    {"role": "user", "content": prompt}
                ],
                max_tokens=600,  # Increased to accommodate better structure
                temperature=0.2  # Reduced for more consistent, focused summaries
            )
            
            content = response.choices[0].message.content
            summary = content.strip() if content else ""
            self.logger.info(f"Summary generated. Length: {len(summary.split())} words")
            return summary
            
        except Exception as e:
            self.logger.error(f"Summary generation failed: {e}")
            raise
    
    def extract_analytics(self, transcription: str, audio_duration: float) -> Dict:
        """Extract analytics from transcription using GPT-4.1-mini."""
        self.logger.info("Extracting analytics...")
        
        word_count = len(transcription.split())
        speaking_speed = word_count / audio_duration if audio_duration > 0 else 0
        
        # First, identify the main topics
        topic_identification_prompt = f"""
        You are an expert text analyst. Analyze the following transcription and identify the 3-5 most prominent topics or themes discussed.

        ## Instructions:
        1. Focus on SUBSTANTIAL topics that are meaningfully discussed (not just mentioned in passing)
        2. Use clear, descriptive topic names (2-4 words each)
        3. Consider topics that span multiple sentences or paragraphs
        4. Avoid overly broad topics like "general discussion" unless truly applicable
        5. Look for recurring themes, concepts, or subjects

        ## Transcription to analyze:
        {transcription}

        ## Response Format:
        Return ONLY a JSON array of topic names:
        ["Topic Name 1", "Topic Name 2", "Topic Name 3"]
        """
        
        try:
            # Get topics first
            topic_response = self.client.chat.completions.create(
                model="gpt-4.1-mini",
                messages=[
                    {"role": "system", "content": "You are an expert text analyst who identifies key topics with precision. Always respond with valid JSON array format."},
                    {"role": "user", "content": topic_identification_prompt}
                ],
                max_tokens=200,
                temperature=0.1
            )
            
            topic_content = topic_response.choices[0].message.content
            topic_content = topic_content.strip() if topic_content else ""
            try:
                identified_topics = json.loads(topic_content)
                if not isinstance(identified_topics, list):
                    identified_topics = ["General Discussion"]
            except json.JSONDecodeError:
                self.logger.warning("Could not parse topics from first response, using fallback")
                identified_topics = ["General Discussion"]
            
            # Now count mentions for each identified topic
            counting_prompt = f"""
            You are an expert text analyst specializing in frequency analysis. For each topic below, count how many times it is mentioned, referenced, or discussed in the transcription.

            ## Counting Rules:
            1. Count DIRECT mentions of the topic by name
            2. Count SYNONYMS and related terms (e.g., "AI" and "artificial intelligence")
            3. Count CONCEPTUAL references (discussing the topic without naming it directly)
            4. Count CONTEXTUAL discussions (extended talk about the topic)
            5. DO NOT count articles, pronouns, or unrelated words that happen to contain the topic word
            6. Be thorough but accurate - reread the text carefully for each topic

            ## Topics to count:
            {json.dumps(identified_topics)}

            ## Transcription to analyze:
            {transcription}

            ## Response Format:
            Return ONLY valid JSON in this exact format:
            {{
                "topics": [
                    {{"topic": "Topic Name", "mentions": actual_count}},
                    {{"topic": "Topic Name", "mentions": actual_count}}
                ]
            }}
            """
            
            # Get mention counts
            count_response = self.client.chat.completions.create(
                model="gpt-4.1-mini",
                messages=[
                    {"role": "system", "content": "You are a meticulous text analyst who counts topic mentions with perfect accuracy. Always respond with valid JSON. Take your time to count carefully."},
                    {"role": "user", "content": counting_prompt}
                ],
                max_tokens=400,
                temperature=0.0  # Maximum determinism for counting
            )
            
            # Parse the counting results
            count_content = count_response.choices[0].message.content
            count_content = count_content.strip() if count_content else ""
            try:
                topics_data = json.loads(count_content)
                topics = topics_data.get("topics", [])
                
                # Validate and clean the results
                valid_topics = []
                for topic in topics:
                    if isinstance(topic, dict) and "topic" in topic and "mentions" in topic:
                        mention_count = topic["mentions"]
                        # Ensure mention count is a positive integer
                        if isinstance(mention_count, (int, float)) and mention_count > 0:
                            valid_topics.append({
                                "topic": str(topic["topic"]),
                                "mentions": int(mention_count)
                            })
                
                # Sort by mention count (descending) and take top 5
                topics = sorted(valid_topics, key=lambda x: x["mentions"], reverse=True)[:5]
                
                # If no valid topics found, use fallback
                if not topics:
                    topics = [{"topic": "General Discussion", "mentions": 1}]
                    
            except json.JSONDecodeError:
                self.logger.warning("Could not parse mention counts, using fallback")
                topics = [{"topic": "General Discussion", "mentions": 1}]
            
            # Create final analytics
            analytics = {
                "word_count": word_count,
                "speaking_speed_wpm": round(speaking_speed, 1),
                "audio_duration_minutes": round(audio_duration, 1),
                "frequently_mentioned_topics": topics,
                "timestamp": datetime.datetime.now().isoformat() + "Z"
            }
            
            self.logger.info(f"Analytics extraction completed. Found {len(topics)} topics.")
            return analytics
            
        except Exception as e:
            self.logger.error(f"Analytics extraction failed: {e}")
            # Return fallback analytics
            return {
                "word_count": word_count,
                "speaking_speed_wpm": round(speaking_speed, 1),
                "audio_duration_minutes": round(audio_duration, 1),
                "frequently_mentioned_topics": [{"topic": "General Discussion", "mentions": 1}],
                "timestamp": datetime.datetime.now().isoformat() + "Z"
            }
    
    def save_results(self, transcription: str, summary: str, analytics: Dict) -> Dict[str, Path]:
        """Save all results to files with timestamp."""
        timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        
        files = {}
        
        # Save transcription
        transcription_file = self.output_dir / f"transcription_{timestamp}.md"
        try:
            with open(transcription_file, 'w', encoding='utf-8') as f:
                f.write(f"# Audio Transcription\n\n")
                f.write(f"**Generated:** {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
                f.write(f"## Transcription\n\n{transcription}\n")
            files['transcription'] = transcription_file
            self.logger.info(f"Transcription saved: {transcription_file}")
        except Exception as e:
            self.logger.error(f"Failed to save transcription: {e}")
        
        # Save summary
        summary_file = self.output_dir / f"summary_{timestamp}.md"
        try:
            with open(summary_file, 'w', encoding='utf-8') as f:
                f.write(f"# Audio Summary\n\n")
                f.write(f"**Generated:** {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
                f.write(f"## Summary\n\n{summary}\n")
            files['summary'] = summary_file
            self.logger.info(f"Summary saved: {summary_file}")
        except Exception as e:
            self.logger.error(f"Failed to save summary: {e}")
        
        # Save analytics
        analytics_file = self.output_dir / f"analysis_{timestamp}.json"
        try:
            with open(analytics_file, 'w', encoding='utf-8') as f:
                json.dump(analytics, f, indent=2, ensure_ascii=False)
            files['analytics'] = analytics_file
            self.logger.info(f"Analytics saved: {analytics_file}")
        except Exception as e:
            self.logger.error(f"Failed to save analytics: {e}")
        
        return files
    
    def process_audio_file(self, file_path: Path) -> Dict[str, Path]:
        """Main method to process an audio file completely."""
        self.logger.info(f"Processing audio file: {file_path}")
        
        # Validate file
        is_valid, message = self.validate_audio_file(file_path)
        if not is_valid:
            raise ValueError(message)
        
        try:
            # Get audio duration
            audio_duration = self.get_audio_duration(file_path)
            self.logger.info(f"Audio duration: {audio_duration:.1f} minutes")
            
            # Transcribe audio
            transcription = self.transcribe_audio(file_path)
            
            # Generate summary
            summary = self.generate_summary(transcription)
            
            # Extract analytics
            analytics = self.extract_analytics(transcription, audio_duration)
            
            # Save results
            output_files = self.save_results(transcription, summary, analytics)
            
            self.logger.info("Processing completed successfully!")
            return output_files
            
        except Exception as e:
            self.logger.error(f"Processing failed: {e}")
            raise


def get_audio_file_path() -> Path:
    """Get audio file path from command line arguments or interactive input."""
    if len(sys.argv) > 1:
        return Path(sys.argv[1])
    
    while True:
        file_path = input("Enter the path to your audio file: ").strip()
        if file_path:
            return Path(file_path)
        print("Please enter a valid file path.")


def main():
    """Main entry point."""
    parser = argparse.ArgumentParser(
        description="Audio Transcription & Analytics Console Application",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python main.py sample_audio.mp3
  python main.py /path/to/audio.wav
  python main.py  # Interactive mode
        """
    )
    parser.add_argument('audio_file', nargs='?', help='Path to audio file to process')
    parser.add_argument('--api-key', help='OpenAI API key (overrides environment variable)')
    
    args = parser.parse_args()
    
    print("🎵 Audio Transcription & Analytics Console Application")
    print("=" * 55)
    
    try:
        # Initialize analyzer
        analyzer = AudioAnalyzer(api_key=args.api_key)
        
        # Get audio file path
        if args.audio_file:
            audio_file = Path(args.audio_file)
        else:
            audio_file = get_audio_file_path()
        
        # Process the audio file
        print(f"\n📁 Processing: {audio_file}")
        print("-" * 40)
        
        output_files = analyzer.process_audio_file(audio_file)
        
        # Display results
        print("\n✅ Processing Complete!")
        print("=" * 25)
        print("\n📄 Generated Files:")
        for file_type, file_path in output_files.items():
            print(f"  • {file_type.title()}: {file_path}")
        
        print(f"\n📂 All files saved in: {analyzer.output_dir.absolute()}")
        
    except KeyboardInterrupt:
        print("\n\n⚠️  Operation cancelled by user.")
        sys.exit(1)
    except Exception as e:
        print(f"\n❌ Error: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main() 