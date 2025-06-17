# Service Analysis Report Generator

A powerful Python console application that generates comprehensive, markdown-formatted analysis reports for digital services and products using OpenAI's GPT-4 API.

## 🌟 Features

- **Dual Input Modes**: Analyze known services by name or provide custom service descriptions
- **Comprehensive Analysis**: Generates reports covering 8 key perspectives:
  - Brief History
  - Target Audience
  - Core Features
  - Unique Selling Points
  - Business Model
  - Tech Stack Insights
  - Perceived Strengths
  - Perceived Weaknesses
- **Flexible Output**: Display results in terminal or save to markdown files
- **Professional Formatting**: Clean, structured markdown output
- **Error Handling**: Robust error handling and user feedback

## 📋 Requirements

- Python 3.7 or higher
- OpenAI API key
- Internet connection

## 🚀 Installation

1. **Clone or download this repository**
   ```bash
   git clone <repository-url>
   cd Task-9
   ```

2. **Install dependencies**
   ```bash
   pip install -r requirements.txt
   ```

3. **Set up your OpenAI API key**
   
   **Option A: Environment Variable (Recommended)**
   ```bash
   # On Windows (PowerShell)
   $env:OPENAI_API_KEY="your-api-key-here"
   
   # On Windows (Command Prompt)
   set OPENAI_API_KEY=your-api-key-here
   
   # On macOS/Linux
   export OPENAI_API_KEY="your-api-key-here"
   ```
   
   **Option B: Command Line Parameter**
   ```bash
   python main.py --api-key "your-api-key-here" --service "Spotify"
   ```
   
   **Option C: Create .env file and set your OpenAI key inside it**
   ```bash
   OPENAI_API_KEY="your-open-ai-key-here"
   ```

## 💡 Usage

The application supports two main modes of operation:

### Mode 1: Analyze Known Services

Analyze well-known digital services by name:

```bash
python main.py --service "Spotify"
python main.py --service "Notion"
python main.py --service "Netflix"
python main.py -s "GitHub"
```

### Mode 2: Analyze Custom Service Descriptions

Provide your own service description text:

```bash
python main.py --text "A cloud-based project management tool that helps teams collaborate, track tasks, and manage deadlines with real-time updates and integrations."
```

```bash
python main.py -t "An AI-powered language learning app that uses personalized lessons, gamification, and speech recognition to help users learn new languages effectively."
```

### Save Reports to Files

Add the `--output` parameter to save reports to markdown files:

```bash
python main.py --service "Spotify" --output spotify_analysis.md
python main.py --text "Your service description here" --output custom_analysis.md
```

## 🔧 Command Line Options

| Option | Short | Description | Required |
|--------|-------|-------------|----------|
| `--service` | `-s` | Name of a known service to analyze | Yes* |
| `--text` | `-t` | Raw service description text to analyze | Yes* |
| `--output` | `-o` | Output file to save the report | No |
| `--api-key` | | OpenAI API key (if not set as env variable) | No** |
| `--help` | `-h` | Show help message and exit | No |

*Either `--service` or `--text` is required (mutually exclusive)
**Required if OPENAI_API_KEY environment variable is not set

## 📖 Example Commands

```bash
# Basic service analysis
python main.py --service "Discord"

# Analyze with custom description
python main.py --text "A subscription-based streaming service offering unlimited access to movies, TV shows, and original content across multiple devices."

# Save analysis to file
python main.py --service "WhatsApp" --output whatsapp_report.md

# Use custom API key
python main.py --api-key "sk-..." --service "Zoom"
```

## 🛠️ Troubleshooting

### Common Issues

**"OpenAI API key is required" Error**
- Ensure your API key is set as an environment variable or passed via `--api-key`
- Verify your API key is valid and has sufficient credits

**Import Errors**
- Make sure you've installed all dependencies: `pip install -r requirements.txt`
- Check your Python version (3.7+ required)

**Network/API Errors**
- Verify your internet connection
- Check if OpenAI services are operational
- Ensure your API key has proper permissions

### Getting Help

Run the application with `--help` to see all available options:
```bash
python main.py --help
```

## 📝 Notes

- The application uses GPT-4 for high-quality analysis
- Analysis accuracy depends on the service's public information availability
- Generated reports are based on publicly available information and AI analysis
- Processing time typically ranges from 10-30 seconds depending on complexity
- Reports are generated in markdown format for easy reading and sharing
