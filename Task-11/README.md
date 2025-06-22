# 🎵 Audio Transcription & Analytics Console Application

A complete console application that processes audio files through OpenAI's APIs to generate transcriptions, summaries, and analytics. This project demonstrates multimodal AI integration and structured data extraction.

## ✨ Features

- **Audio Transcription**: Uses OpenAI Whisper API (whisper-1 model) for high-quality transcription
- **Text Summarization**: Generates concise summaries using GPT-4.1-mini
- **Analytics Extraction**: Calculates word count, speaking speed, and identifies key topics
- **Multi-format Support**: Handles MP3, WAV, M4A, MP4, and other common audio formats
- **Automatic Language Detection**: Whisper API automatically detects the language
- **Structured Output**: Saves results in markdown and JSON formats with timestamps

## 🛠️ Technical Stack

- **Language**: Python 3.8+
- **APIs**: OpenAI Whisper API (whisper-1) and GPT-4.1-mini
- **Dependencies**: openai, mutagen
- **Output Formats**: Markdown (.md) and JSON (.json)

## 📋 Prerequisites

1. **Python 3.8 or higher**
2. **OpenAI API key** with access to:
   - Whisper API (for transcription)
   - GPT-4.1-mini (for summaries and analytics)

## 🚀 Installation

### 1. Clone or Download the Project
```bash
git clone https://github.com/Ilxom-22/edu-ai-challenge-2025.git
cd Task-11
```

### 2. Install Dependencies
```bash
pip install -r requirements.txt
```

### 3. Set Up OpenAI API Key

#### Option A: Environment Variable (Recommended)
**Windows:**
```cmd
set OPENAI_API_KEY=your_actual_api_key_here
```

**macOS/Linux:**
```bash
export OPENAI_API_KEY=your_actual_api_key_here
```

#### Option B: Command Line Parameter
```bash
python main.py --api-key your_actual_api_key_here sample_audio.mp3
```

### 4. Get Your OpenAI API Key
1. Visit [OpenAI Platform](https://platform.openai.com/api-keys)
2. Sign in to your account
3. Create a new API key
4. Copy the key and use it in step 3 above

## 📖 Usage

### Command Line with File Path
```bash
python main.py sample_audio.mp3
```

### Interactive Mode
```bash
python main.py
# The application will prompt you for the audio file path
```

### With Custom API Key
```bash
python main.py --api-key your_api_key sample_audio.mp3
```

### Help and Options
```bash
python main.py --help
```

## 📁 Output Files

The application creates an `outputs/` directory and generates three files with timestamps:

### 1. Transcription (`transcription_YYYYMMDD_HHMMSS.md`)
```markdown
# Audio Transcription

**Generated:** 2024-12-15 14:30:00

## Transcription

[Full transcription text here...]
```

### 2. Summary (`summary_YYYYMMDD_HHMMSS.md`)
```markdown
# Audio Summary

**Generated:** 2024-12-15 14:30:00

## Summary

[Concise summary preserving key points and main takeaways...]
```

### 3. Analytics (`analysis_YYYYMMDD_HHMMSS.json`)
```json
{
  "word_count": 1280,
  "speaking_speed_wpm": 132,
  "audio_duration_minutes": 9.7,
  "frequently_mentioned_topics": [
    { "topic": "Customer Onboarding", "mentions": 6 },
    { "topic": "Q4 Roadmap", "mentions": 4 },
    { "topic": "AI Integration", "mentions": 3 }
  ],
  "timestamp": "2024-12-15T14:30:00Z"
}
```

## 🎯 Supported Audio Formats

- MP3 (.mp3)
- WAV (.wav)
- M4A (.m4a)
- MP4 (.mp4)
- MPEG (.mpeg)
- MPGA (.mpga)
- WebM (.webm)

**File Size Limit**: 25MB (OpenAI Whisper API limitation)

## ⚡ Example Usage

### Basic Processing
```bash
# Process the sample file
python main.py sample_audio.mp3

# Output:
🎵 Audio Transcription & Analytics Console Application
=======================================================

📁 Processing: sample_audio.mp3
----------------------------------------
2024-12-15 14:30:00 - INFO - Output directory: /path/to/outputs
2024-12-15 14:30:00 - INFO - Audio duration: 9.7 minutes
2024-12-15 14:30:00 - INFO - Starting audio transcription...
2024-12-15 14:30:15 - INFO - Transcription completed. Length: 2156 characters
2024-12-15 14:30:15 - INFO - Generating summary...
2024-12-15 14:30:25 - INFO - Summary generated. Length: 187 words
2024-12-15 14:30:25 - INFO - Extracting analytics...
2024-12-15 14:30:35 - INFO - Analytics extraction completed
2024-12-15 14:30:35 - INFO - Transcription saved: outputs/transcription_20241215_143035.md
2024-12-15 14:30:35 - INFO - Summary saved: outputs/summary_20241215_143035.md
2024-12-15 14:30:35 - INFO - Analytics saved: outputs/analysis_20241215_143035.json

✅ Processing Complete!
=========================

📄 Generated Files:
  • Transcription: outputs/transcription_20241215_143035.md
  • Summary: outputs/summary_20241215_143035.md
  • Analytics: outputs/analysis_20241215_143035.json

📂 All files saved in: /absolute/path/to/outputs
```

## 🔧 Troubleshooting

### Common Issues

#### 1. "Missing required dependencies" Error
**Solution:**
```bash
pip install -r requirements.txt
```

#### 2. "OpenAI API key not found" Error
**Solutions:**
- Set the environment variable: `set OPENAI_API_KEY=your_key` (Windows) or `export OPENAI_API_KEY=your_key` (macOS/Linux)
- Use the `--api-key` parameter: `python main.py --api-key your_key file.mp3`
- Check that your API key is valid at [OpenAI Platform](https://platform.openai.com/api-keys)

#### 3. "File too large" Error
**Solution:** The file exceeds the 25MB limit. Try:
- Using a shorter audio clip
- Compressing the audio file
- Converting to a more efficient format (MP3)

#### 4. "Unsupported format" Error
**Solution:** Convert your audio to a supported format:
- MP3, WAV, M4A, MP4, MPEG, MPGA, or WebM

#### 5. "Rate limit exceeded" Error
**Solution:**
- Wait a few minutes and try again
- Check your OpenAI usage limits
- Consider upgrading your OpenAI plan

#### 6. "Permission denied creating output directory" Error
**Solution:**
- Run the application from a directory where you have write permissions
- Check folder permissions
- Try running as administrator (Windows) or with sudo (macOS/Linux)

### API Settings
- **Whisper Model**: whisper-1 (automatic language detection)
- **GPT Model**: gpt-4.1-mini (for summaries and analytics)
- **Summary Length**: 150-300 words (adaptive based on input length)
- **Temperature**: 0.3 (summaries), 0.1 (analytics)

## 🧪 Testing

### Test with Different Audio Types
```bash
# Test with different formats
python main.py test_audio.mp3
python main.py meeting_recording.wav
python main.py podcast_episode.m4a

# Test with different languages (auto-detected)
python main.py spanish_audio.mp3
python main.py french_meeting.wav
```

### Validate Output Structure
The JSON analytics output follows this exact schema:
```json
{
  "word_count": number,
  "speaking_speed_wpm": number,
  "audio_duration_minutes": number,
  "frequently_mentioned_topics": [
    { "topic": string, "mentions": number }
  ],
  "timestamp": "ISO 8601 string"
}
```

**Happy transcribing! 🎉** 