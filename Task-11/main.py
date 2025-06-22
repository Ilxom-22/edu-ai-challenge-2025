#!/usr/bin/env python3
"""
Audio Transcription & Analytics Console Application

This application processes audio files through OpenAI's APIs to generate
transcriptions, summaries, and analytics with full multilingual support.
"""

import os
import sys
import argparse
import json
import datetime
import re
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
    import numpy as np
    import jieba
except ImportError as e:
    print(f"Missing required dependencies. Please run: pip install -r requirements.txt")
    print(f"Error: {e}")
    sys.exit(1)


class MultilingualTextProcessor:
    """Handles multilingual text processing and language-aware analytics."""
    
    # Speaking speed ranges by language (words per minute)
    LANGUAGE_SPEED_RANGES = {
        'en': (130, 180),  # English
        'es': (140, 200),  # Spanish
        'fr': (120, 170),  # French
        'de': (120, 160),  # German
        'it': (130, 180),  # Italian
        'pt': (140, 190),  # Portuguese
        'ru': (110, 150),  # Russian
        'zh': (80, 120),   # Chinese (characters, not words)
        'ja': (90, 130),   # Japanese (characters, not words)
        'ko': (100, 140),  # Korean
        'ar': (120, 160),  # Arabic
        'hi': (110, 150),  # Hindi
        'default': (100, 160)  # Default range
    }
    
    @staticmethod
    def detect_language_with_ai(text: str, client: OpenAI) -> str:
        """AI-powered language detection for better accuracy."""
        if not text or not text.strip():
            return 'en'
            
        try:
            prompt = f"""
            Detect the primary language of the following text. Respond with only the 2-letter ISO 639-1 language code.
            
            Examples:
            - English → en
            - Spanish → es  
            - French → fr
            - German → de
            - Chinese → zh
            - Japanese → ja
            - Russian → ru
            - Arabic → ar
            - Portuguese → pt
            - Italian → it
            - Korean → ko
            - Hindi → hi
            
            If the text contains multiple languages, identify the dominant one.
            If you're uncertain, provide your best assessment.
            
            Text to analyze:
            {text[:1000]}
            
            Language code:
            """
            
            response = client.chat.completions.create(
                model="gpt-4.1-mini",
                messages=[
                    {"role": "system", "content": "You are an expert linguist. Respond with only the 2-letter ISO language code, nothing else."},
                    {"role": "user", "content": prompt}
                ],
                max_tokens=5,
                temperature=0.1
            )
            
            # Fix potential None attribute error
            content = response.choices[0].message.content
            detected_lang = content.strip().lower() if content else 'en'
            
            # Validate response is a proper 2-letter code
            if detected_lang and len(detected_lang) == 2 and detected_lang.isalpha():
                return detected_lang
            else:
                return 'en'  # Fallback to English
                
        except Exception as e:
            return 'en'  # Default to English on any error
    
    @staticmethod
    def count_words_multilingual(text: str, language: str) -> int:
        """Language-aware word counting."""
        if not text or not text.strip():
            return 0
            
        # Chinese and Japanese use characters, not spaces
        if language in ['zh', 'zh-cn', 'zh-tw']:
            # Use jieba for Chinese word segmentation
            words = jieba.lcut(text)
            return len(words)
        elif language in ['ja']:
            # For Japanese, count characters as rough approximation
            # Remove spaces and count non-whitespace characters
            chars = re.sub(r'\s+', '', text)
            return len(chars) // 2  # Rough approximation to word count
        elif language in ['ar', 'he']:
            # Arabic and Hebrew - split by spaces but handle RTL
            words = text.split()
            return len([w for w in words if w.strip()])
        else:
            # For most languages, split by whitespace
            words = text.split()
            return len([w for w in words if w.strip()])
    
    @staticmethod
    def get_language_name(lang_code: str) -> str:
        """Get human-readable language name with enhanced mapping."""
        # Handle None input
        if lang_code is None:
            return 'Detected (unknown)'
        
        # Normalize input (handle both ISO codes and full names)
        lang_code = str(lang_code).lower().strip()
        
        # Comprehensive language mapping
        lang_mapping = {
            # ISO 639-1 codes
            'en': 'English', 'es': 'Spanish', 'fr': 'French', 'de': 'German',
            'it': 'Italian', 'pt': 'Portuguese', 'ru': 'Russian', 'zh': 'Chinese',
            'ja': 'Japanese', 'ko': 'Korean', 'ar': 'Arabic', 'hi': 'Hindi',
            'nl': 'Dutch', 'sv': 'Swedish', 'da': 'Danish', 'no': 'Norwegian',
            'fi': 'Finnish', 'pl': 'Polish', 'tr': 'Turkish', 'th': 'Thai',
            'vi': 'Vietnamese', 'id': 'Indonesian', 'ms': 'Malay', 'uk': 'Ukrainian',
            'he': 'Hebrew', 'fa': 'Persian', 'ur': 'Urdu', 'bn': 'Bengali',
            'ta': 'Tamil', 'te': 'Telugu', 'mr': 'Marathi', 'gu': 'Gujarati',
            'kn': 'Kannada', 'ml': 'Malayalam', 'pa': 'Punjabi', 'or': 'Odia',
            'as': 'Assamese', 'ne': 'Nepali', 'si': 'Sinhala', 'my': 'Myanmar',
            'km': 'Khmer', 'lo': 'Lao', 'ka': 'Georgian', 'am': 'Amharic',
            'sw': 'Swahili', 'zu': 'Zulu', 'af': 'Afrikaans', 'sq': 'Albanian',
            'eu': 'Basque', 'be': 'Belarusian', 'bg': 'Bulgarian', 'ca': 'Catalan',
            'hr': 'Croatian', 'cs': 'Czech', 'et': 'Estonian', 'gl': 'Galician',
            'hu': 'Hungarian', 'is': 'Icelandic', 'ga': 'Irish', 'lv': 'Latvian',
            'lt': 'Lithuanian', 'mk': 'Macedonian', 'mt': 'Maltese', 'ro': 'Romanian',
            'sk': 'Slovak', 'sl': 'Slovenian', 'sr': 'Serbian', 'cy': 'Welsh',
            
            # Full language names (sometimes returned by Whisper)
            'english': 'English', 'spanish': 'Spanish', 'french': 'French', 
            'german': 'German', 'italian': 'Italian', 'portuguese': 'Portuguese',
            'russian': 'Russian', 'chinese': 'Chinese', 'japanese': 'Japanese',
            'korean': 'Korean', 'arabic': 'Arabic', 'hindi': 'Hindi',
            'dutch': 'Dutch', 'swedish': 'Swedish', 'danish': 'Danish',
            'norwegian': 'Norwegian', 'finnish': 'Finnish', 'polish': 'Polish',
            'turkish': 'Turkish', 'thai': 'Thai', 'vietnamese': 'Vietnamese',
            'indonesian': 'Indonesian', 'malay': 'Malay', 'ukrainian': 'Ukrainian',
            'hebrew': 'Hebrew', 'persian': 'Persian', 'urdu': 'Urdu'
        }
        
        return lang_mapping.get(lang_code, f'Detected ({lang_code})')
    
    @staticmethod
    def get_universal_prompt(language: str, language_name: str, prompt_type: str, **kwargs) -> str:
        """Generate universal English prompts that guide LLM to respond in the detected language."""
        
        if prompt_type == 'summary':
            target_length = kwargs.get('target_length', 200)
            transcription = kwargs.get('transcription', '')
            
            return f"""
            You are an expert summarization specialist. Create a comprehensive yet concise summary of the following transcription.
            
            IMPORTANT: Respond entirely in {language_name} ({language}), the same language as the input text.
            
            ## Instructions:
            1. Target length: approximately {target_length} units (words for most languages, characters for Chinese/Japanese)
            2. Structure your summary with clear sections appropriate for {language_name}
            3. Capture the main themes, key points, and important details
            4. Preserve critical information, data, names, and conclusions
            5. Use clear, professional {language_name} language with proper grammar and cultural context
            6. Include actionable items or next steps if mentioned
            7. Maintain the logical flow of the original content
            8. Use formatting and structure appropriate for {language_name} readers

            ## Summary Format (adapt to {language_name} conventions):
            **Main Topic/Purpose:** [Brief description in {language_name}]

            **Key Points:**
            • [Most important point 1 in {language_name}]
            • [Most important point 2 in {language_name}]
            • [Most important point 3 in {language_name}]

            **Details & Context:** 
            [Elaborate on the key points with supporting details in {language_name}]

            **Conclusions/Outcomes:** 
            [Any decisions made, conclusions reached, or next steps identified in {language_name}]

            ## Transcription to Summarize:
            {transcription}

            ## Summary (respond in {language_name}):
            """
            
        elif prompt_type == 'topic_identification':
            transcription = kwargs.get('transcription', '')
            
            return f"""
            You are an expert text analyst. Analyze the following transcription and identify the 3-5 most prominent topics or themes discussed.
            
            IMPORTANT: Provide topic names in {language_name} ({language}), the same language as the input text.

            ## Instructions:
            1. Focus on SUBSTANTIAL topics that are meaningfully discussed (not just mentioned in passing)
            2. Use clear, descriptive topic names (2-4 words each) in {language_name}
            3. Consider topics that span multiple sentences or paragraphs
            4. Avoid overly broad topics like "general discussion" unless truly applicable
            5. Look for recurring themes, concepts, or subjects
            6. Provide culturally appropriate topic names for {language_name} speakers

            ## Transcription to analyze:
            {transcription}

            ## Response Format:
            Return ONLY a JSON array of topic names in {language_name}:
            ["Topic Name 1", "Topic Name 2", "Topic Name 3"]
            """
            
        elif prompt_type == 'topic_counting':
            identified_topics = kwargs.get('identified_topics', [])
            transcription = kwargs.get('transcription', '')
            
            return f"""
            You are an expert text analyst specializing in frequency analysis. For each topic below, count how many times it is mentioned, referenced, or discussed in the transcription.
            
            The topics are in {language_name} ({language}). Analyze the text carefully in its original language.

            ## Counting Rules:
            1. Count DIRECT mentions of the topic by name
            2. Count SYNONYMS and related terms in {language_name}
            3. Count CONCEPTUAL references (discussing the topic without naming it directly)
            4. Count CONTEXTUAL discussions (extended talk about the topic)
            5. DO NOT count articles, pronouns, or unrelated words
            6. Be thorough but accurate - reread the text carefully for each topic
            7. Consider cultural and linguistic variations in {language_name}

            ## Topics to count:
            {json.dumps(identified_topics, ensure_ascii=False)}

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
        
        # Default fallback
        return ""


class AudioAnalyzer:
    """Main class for audio transcription and analytics with multilingual support."""
    
    SUPPORTED_FORMATS = {'.mp3', '.wav', '.m4a', '.mp4', '.mpeg', '.mpga', '.webm'}
    MAX_FILE_SIZE = 25 * 1024 * 1024  # 25MB limit for Whisper API
    
    def __init__(self, api_key: Optional[str] = None):
        """Initialize the AudioAnalyzer with OpenAI API key."""
        self.api_key = api_key or os.getenv('OPENAI_API_KEY')
        if not self.api_key:
            raise ValueError("OpenAI API key not found. Please set OPENAI_API_KEY environment variable.")
        
        self.client = OpenAI(api_key=self.api_key)
        self.text_processor = MultilingualTextProcessor()
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
        
        return True, "Valid audio file"
    
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
                # For other formats, try generic mutagen detection
                try:
                    from mutagen import File
                    audio = File(file_path)
                    if audio is None:
                        self.logger.warning("Could not determine audio format")
                        return 0.0
                except ImportError:
                    self.logger.warning("Could not import mutagen.File")
                    return 0.0
            
            if audio is not None and hasattr(audio, 'info') and hasattr(audio.info, 'length'):
                return audio.info.length / 60.0  # Convert to minutes
            else:
                self.logger.warning("Could not determine audio duration")
                return 0.0
        except Exception as e:
            self.logger.warning(f"Error getting audio duration: {e}")
            return 0.0
    
    def transcribe_audio(self, file_path: Path) -> Tuple[str, str]:
        """Transcribe audio using OpenAI Whisper API with language detection."""
        self.logger.info("Starting audio transcription...")
        
        try:
            with open(file_path, 'rb') as audio_file:
                # Use verbose_json to get language information
                response = self.client.audio.transcriptions.create(
                    model="whisper-1",
                    file=audio_file,
                    response_format="verbose_json"
                )
            
            transcription = response.text.strip() if response.text else ""
            detected_language = response.language if hasattr(response, 'language') else 'en'
            
            # Fallback language detection using AI text analysis
            if not detected_language or detected_language == 'en':
                detected_language = self.text_processor.detect_language_with_ai(transcription, self.client)
            
            self.logger.info(f"Transcription completed. Language: {self.text_processor.get_language_name(detected_language)}, Length: {len(transcription)} characters")
            return transcription, detected_language
            
        except openai.RateLimitError:
            self.logger.error("Rate limit exceeded. Please wait and try again.")
            raise
        except openai.AuthenticationError:
            self.logger.error("Invalid API key. Please check your OpenAI API key.")
            raise
        except Exception as e:
            self.logger.error(f"Transcription failed: {e}")
            raise
    
    def generate_summary(self, transcription: str, language: str) -> str:
        """Generate summary using GPT-4.1-mini with universal prompts."""
        self.logger.info("Generating summary...")
        
        word_count = self.text_processor.count_words_multilingual(transcription, language)
        target_length = min(300, max(150, word_count // 10))
        language_name = self.text_processor.get_language_name(language)
        
        prompt = self.text_processor.get_universal_prompt(language, language_name, 'summary', transcription=transcription, target_length=target_length)
        
        try:
            response = self.client.chat.completions.create(
                model="gpt-4.1-mini",
                messages=[
                    {"role": "system", "content": "You are an expert content summarizer who creates structured, informative summaries that capture both the essence and important details of any text. Always follow the requested format and maintain accuracy. Respond in the same language as the input text."},
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
    
    def extract_analytics(self, transcription: str, audio_duration: float, language: str) -> Dict:
        """Extract analytics from transcription using GPT-4.1-mini with universal prompts."""
        self.logger.info("Extracting analytics...")
        
        word_count = self.text_processor.count_words_multilingual(transcription, language)
        speaking_speed = round(word_count / audio_duration, 0) if audio_duration > 0 else 0
        language_name = self.text_processor.get_language_name(language)
        
        # First, identify the main topics
        topic_identification_prompt = self.text_processor.get_universal_prompt(language, language_name, 'topic_identification', transcription=transcription)
        
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
            counting_prompt = self.text_processor.get_universal_prompt(language, language_name, 'topic_counting', identified_topics=identified_topics, transcription=transcription)
            
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
            
            # Create final analytics with the requested format
            analytics = {
                "language": language_name,
                "word_count": word_count,
                "speaking_speed_wpm": int(speaking_speed),
                "audio_duration_minutes": round(audio_duration, 1),
                "timestamp": datetime.datetime.now().isoformat() + "Z",
                "frequently_mentioned_topics": topics
            }
            
            self.logger.info(f"Analytics extraction completed. Language: {language_name}, Found {len(topics)} topics.")
            return analytics
            
        except Exception as e:
            self.logger.error(f"Analytics extraction failed: {e}")
            # Return fallback analytics
            return {
                "language": language_name,
                "word_count": word_count,
                "speaking_speed_wpm": int(speaking_speed),
                "audio_duration_minutes": round(audio_duration, 1),
                "timestamp": datetime.datetime.now().isoformat() + "Z",
                "frequently_mentioned_topics": [{"topic": "General Discussion", "mentions": 1}]
            }
    
    def save_results(self, transcription: str, summary: str, analytics: Dict, language: str) -> Dict[str, Path]:
        """Save all results to files with timestamp and language information."""
        timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        language_name = self.text_processor.get_language_name(language)
        
        files = {}
        
        # Save transcription
        transcription_file = self.output_dir / f"transcription_{language}_{timestamp}.md"
        try:
            with open(transcription_file, 'w', encoding='utf-8') as f:
                f.write(f"# Audio Transcription\n\n")
                f.write(f"**Generated:** {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
                f.write(f"**Language:** {language_name} ({language})\n\n")
                f.write(f"## Transcription\n\n{transcription}\n")
            files['transcription'] = transcription_file
            self.logger.info(f"Transcription saved: {transcription_file}")
        except Exception as e:
            self.logger.error(f"Failed to save transcription: {e}")
        
        # Save summary
        summary_file = self.output_dir / f"summary_{language}_{timestamp}.md"
        try:
            with open(summary_file, 'w', encoding='utf-8') as f:
                f.write(f"# Audio Summary\n\n")
                f.write(f"**Generated:** {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
                f.write(f"**Language:** {language_name} ({language})\n\n")
                f.write(f"## Summary\n\n{summary}\n")
            files['summary'] = summary_file
            self.logger.info(f"Summary saved: {summary_file}")
        except Exception as e:
            self.logger.error(f"Failed to save summary: {e}")
        
        # Save analytics
        analytics_file = self.output_dir / f"analysis_{language}_{timestamp}.json"
        try:
            with open(analytics_file, 'w', encoding='utf-8') as f:
                json.dump(analytics, f, indent=2, ensure_ascii=False)
            files['analytics'] = analytics_file
            self.logger.info(f"Analytics saved: {analytics_file}")
        except Exception as e:
            self.logger.error(f"Failed to save analytics: {e}")
        
        return files
    
    def process_audio_file(self, file_path: Path) -> Dict[str, Path]:
        """Main method to process an audio file completely with multilingual support."""
        self.logger.info(f"Processing audio file: {file_path}")
        
        # Validate file
        is_valid, message = self.validate_audio_file(file_path)
        if not is_valid:
            raise ValueError(message)
        
        self.logger.info(f"File validation: {message}")
        
        try:
            # Get audio duration
            audio_duration = self.get_audio_duration(file_path)
            self.logger.info(f"Audio duration: {audio_duration:.1f} minutes")
            
            # Transcribe audio with language detection
            transcription, language = self.transcribe_audio(file_path)
            
            # Generate summary with universal prompts
            summary = self.generate_summary(transcription, language)
            
            # Extract analytics with multilingual support
            analytics = self.extract_analytics(transcription, audio_duration, language)
            
            # Save results with language information
            output_files = self.save_results(transcription, summary, analytics, language)
            
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
        description="Audio Transcription & Analytics Console Application with Multilingual Support",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python main.py sample_audio.mp3
  python main.py /path/to/audio.wav
  python main.py  # Interactive mode

Supported Languages:
  - English, Spanish, French, German, Italian, Portuguese
  - Chinese, Japanese, Korean, Arabic, Hindi, Russian
  - And many more languages supported by OpenAI Whisper

Features:
  - AI-powered language detection
  - Language-aware word counting and analytics
  - Universal prompts for consistent multilingual output
  - Enhanced topic analysis with cultural context
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