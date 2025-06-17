#!/usr/bin/env python3
"""
Service Analysis Report Generator

A console application that generates comprehensive markdown reports 
about digital services using OpenAI's GPT-4 API.
"""

import os
import sys
import argparse
from typing import Optional
from openai import OpenAI
from datetime import datetime
from dotenv import load_dotenv
load_dotenv()


class ServiceAnalyzer:
    """Main class for analyzing services and generating reports."""
    
    def __init__(self, api_key: Optional[str] = None):
        """Initialize the service analyzer with OpenAI API key."""
        if api_key:
            self.api_key = api_key
        else:
            self.api_key = os.getenv('OPENAI_API_KEY')
        
        if not self.api_key:
            raise ValueError(
                "OpenAI API key is required. Set OPENAI_API_KEY environment variable "
                "or provide it as a parameter."
            )
        
        self.client = OpenAI(api_key=self.api_key)
    
    def generate_prompt(self, input_text: str, is_service_name: bool) -> str:
        """Generate the prompt for OpenAI API based on input type."""
        
        base_sections = """
        ## Brief History
        - Founding year and key milestones
        - Important developments and timeline
        
        ## Target Audience
        - Primary user segments
        - Demographics and use cases
        
        ## Core Features
        - Top 2-4 key functionalities
        - Main capabilities users rely on
        
        ## Unique Selling Points
        - Key differentiators from competitors
        - What makes this service stand out
        
        ## Business Model
        - How the service generates revenue
        - Pricing strategy and monetization
        
        ## Tech Stack Insights
        - Technologies likely used (based on service type)
        - Technical architecture hints
        
        ## Perceived Strengths
        - Standout features and advantages
        - What users typically praise
        
        ## Perceived Weaknesses
        - Common criticisms or limitations
        - Areas for potential improvement
        """
        
        if is_service_name:
            prompt = f"""
            You are an expert business and technology analyst. I need you to generate a comprehensive analysis report about the service "{input_text}".
            
            Please provide a detailed markdown-formatted report covering the following sections:
            {base_sections}
            
            Requirements:
            - Use proper markdown formatting with headers (##)
            - Provide accurate, well-researched information
            - Each section should be substantive (2-4 bullet points minimum)
            - Be objective and balanced in your analysis
            - If you're unsure about specific details, indicate this clearly
            
            Service to analyze: {input_text}
            """
        else:
            prompt = f"""
            You are an expert business and technology analyst. I'm providing you with a description of a digital service/product. Please analyze this information and generate a comprehensive markdown report.
            
            Service Description:
            {input_text}
            
            Please provide a detailed markdown-formatted report covering the following sections:
            {base_sections}
            
            Requirements:
            - Use proper markdown formatting with headers (##)
            - Base your analysis on the provided description
            - Each section should be substantive (2-4 bullet points minimum)
            - Make reasonable inferences where appropriate
            - Be objective and balanced in your analysis
            - If information is not available from the description, make educated assumptions and note them
            
            Focus on extracting insights from the provided text and filling in reasonable details for a complete analysis.
            """
        
        return prompt
    
    def analyze_service(self, input_text: str, is_service_name: bool = True) -> str:
        """Analyze the service and generate a comprehensive report."""
        
        try:
            prompt = self.generate_prompt(input_text, is_service_name)
            
            print("🔍 Analyzing service and generating report...")
            print("⏳ This may take a few moments...\n")
            
            response = self.client.chat.completions.create(
                model="gpt-4.1-mini",  # Using GPT-4 as requested
                messages=[
                    {"role": "system", "content": "You are an expert business and technology analyst who provides comprehensive, accurate, and well-structured reports."},
                    {"role": "user", "content": prompt}
                ],
                max_tokens=2000,
                temperature=0.3  # Lower temperature for more consistent, factual responses
            )
            
            return response.choices[0].message.content
            
        except Exception as e:
            return f"❌ Error generating report: {str(e)}"
    
    def save_report(self, report: str, filename: str) -> bool:
        """Save the report to a file."""
        try:
            with open(filename, 'w', encoding='utf-8') as f:
                f.write(f"# Service Analysis Report\n")
                f.write(f"Generated on: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
                f.write(report)
            return True
        except Exception as e:
            print(f"❌ Error saving report: {str(e)}")
            return False


def main():
    """Main function to run the console application."""
    
    parser = argparse.ArgumentParser(
        description="Generate comprehensive analysis reports for digital services",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python main.py --service "Spotify"
  python main.py --text "A cloud-based project management tool for teams..."
  python main.py --service "Notion" --output report.md
        """
    )
    
    # Create mutually exclusive group for input type
    input_group = parser.add_mutually_exclusive_group(required=True)
    input_group.add_argument(
        '--service', '-s',
        type=str,
        help='Name of a known service (e.g., "Spotify", "Notion")'
    )
    input_group.add_argument(
        '--text', '-t',
        type=str,
        help='Raw service description text'
    )
    
    parser.add_argument(
        '--output', '-o',
        type=str,
        help='Output file to save the report (optional)'
    )
    
    parser.add_argument(
        '--api-key',
        type=str,
        help='OpenAI API key (optional if OPENAI_API_KEY env var is set)'
    )
    
    args = parser.parse_args()
    
    try:
        # Initialize analyzer
        analyzer = ServiceAnalyzer(api_key=args.api_key)
        
        # Determine input type and content
        if args.service:
            input_text = args.service
            is_service_name = True
            print(f"📋 Analyzing known service: {input_text}")
        else:
            input_text = args.text
            is_service_name = False
            print(f"📋 Analyzing provided service description")
        
        # Generate report
        report = analyzer.analyze_service(input_text, is_service_name)
        
        # Display report
        print("✅ Report generated successfully!\n")
        print("=" * 60)
        print(report)
        print("=" * 60)
        
        # Save to file if requested
        if args.output:
            if analyzer.save_report(report, args.output):
                print(f"\n💾 Report saved to: {args.output}")
            else:
                print(f"\n❌ Failed to save report to: {args.output}")
    
    except ValueError as e:
        print(f"❌ Configuration Error: {e}")
        print("\n💡 Tip: Set your OpenAI API key as an environment variable:")
        print("   export OPENAI_API_KEY='your-api-key-here'")
        print("   or use --api-key parameter")
        sys.exit(1)
    
    except KeyboardInterrupt:
        print("\n\n⏹️  Operation cancelled by user.")
        sys.exit(0)
    
    except Exception as e:
        print(f"❌ Unexpected error: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main() 