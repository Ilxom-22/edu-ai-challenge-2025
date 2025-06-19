# Natural Language Product Search Tool

A console-based product search application that uses OpenAI's function calling capability to filter products from a dataset using natural language input.

## 🚀 Features

- **Natural Language Processing**: Enter search queries in plain English
- **AI-Powered Filtering**: Uses OpenAI's function calling to parse and filter products
- **Multiple Filter Types**: Category, price range, rating, stock status, and keywords
- **Clean Console Interface**: User-friendly command-line experience
- **Structured Output**: Displays both AI-extracted criteria and filtered results

## 📋 Requirements

- Python 3.7+
- OpenAI API key
- Internet connection for API calls

## 🛠️ Setup Instructions

### 1. Clone or Download the Project

```bash
# If using git
git clone <your-repo-url>
cd Task-10

# Or download and extract the project files
```

### 2. Install Dependencies

```bash
pip install -r requirements.txt
```

### 3. Configure Environment Variables

1. Copy the example environment file:
   ```bash
   cp .env.example .env
   ```

2. Edit the `.env` file and replace `your_openai_api_key_here` with your actual API key:
   ```
   OPENAI_API_KEY=sk-your-actual-api-key-here
   ```

⚠️ **Important**: Never commit your `.env` file to version control. It contains sensitive information.

## 🎯 How to Run the App

### Start the Application

```bash
python main.py
```

### Using the Search Tool

1. The app will display a welcome message and examples
2. Enter your search query in natural language
3. The AI will process your request and show:
   - Extracted search criteria
   - Filtered products matching your requirements
4. Type `quit`, `exit`, or `q` to exit the application

### Example Queries

- `"Show me in-stock electronics under $300 with at least 4 stars"`
- `"I want fitness equipment below $150"`
- `"Find me kitchen appliances over $50"`
- `"Show me books under $30 with good ratings"`
- `"I need a smartphone or headphones that's in stock"`

## 📁 Project Structure

```
Task-10/
├── main.py              # Main console application
├── functions.py         # OpenAI function schemas and filtering logic
├── products.json        # Product database (50 sample products)
├── requirements.txt     # Python dependencies
├── .env.example        # Environment variables template
├── readme.md           # This file
└── sample_outputs.md   # Example application runs
```

## 🔧 Technical Details

### How It Works

1. **User Input**: Natural language query entered via console
2. **AI Processing**: OpenAI GPT-4.1-mini processes the query using function calling
3. **Criteria Extraction**: AI determines filtering criteria (category, price, rating, etc.)
4. **Product Filtering**: Python applies the extracted criteria to the product dataset
5. **Results Display**: Formatted output shows both criteria and matching products

### Key Components

- **OpenAI Function Calling**: Handles natural language parsing and criteria extraction
- **Structured Filtering**: Applies multiple filter types (category, price, rating, stock, keywords)
- **Error Handling**: Graceful handling of API errors and invalid inputs
- **Clean UI**: Formatted console output with clear sections and indicators

## 🔍 Available Product Categories

- **Electronics**: Headphones, laptops, smartphones, monitors, etc.
- **Fitness**: Exercise equipment, yoga mats, weights, etc.
- **Kitchen**: Appliances, cookware, small electronics
- **Books**: Fiction, non-fiction, guides, children's books
- **Clothing**: Apparel and accessories for men and women

## 💡 Tips for Better Results

1. **Be Specific**: More detailed queries yield better results
2. **Use Natural Language**: Write as you would speak
3. **Combine Criteria**: Mix category, price, and rating requirements
4. **Check Stock Status**: Specify if you want only in-stock items
5. **Use Keywords**: Mention specific product features or names

## 🐛 Troubleshooting

### Common Issues

1. **API Key Error**: 
   - Ensure your `.env` file exists and contains a valid OpenAI API key
   - Check that the key starts with `sk-`

2. **No Results Found**:
   - Try broader search criteria
   - Check if your price range or rating requirements are too restrictive

3. **Connection Errors**:
   - Verify internet connection
   - Check if OpenAI API is accessible from your network

### Getting Help

If you encounter issues:
1. Check the error message displayed by the application
2. Verify your OpenAI API key is correct and active
3. Ensure all required files are present in the project directory
4. Check that dependencies are properly installed