#!/usr/bin/env python3
"""
Natural Language Product Search Tool
A console application that uses OpenAI function calling to filter products based on natural language input.
"""

import json
import os
import sys
from dotenv import load_dotenv
from openai import OpenAI
from functions import get_search_function_schema, filter_products_by_criteria

# Load environment variables
load_dotenv()

class ProductSearchTool:
    def __init__(self):
        """Initialize the product search tool with OpenAI client and product data."""
        # Initialize OpenAI client
        api_key = os.getenv('OPENAI_API_KEY')
        if not api_key:
            print("❌ Error: OPENAI_API_KEY not found in environment variables.")
            print("Please create a .env file with your OpenAI API key.")
            print("See .env.example for reference.")
            sys.exit(1)
        
        self.client = OpenAI(api_key=api_key)
        
        # Load product data
        try:
            with open('products.json', 'r') as f:
                self.products = json.load(f)
            print(f"✅ Loaded {len(self.products)} products from products.json")
        except FileNotFoundError:
            print("❌ Error: products.json file not found.")
            sys.exit(1)
        except json.JSONDecodeError:
            print("❌ Error: Invalid JSON in products.json file.")
            sys.exit(1)
    
    def search_products(self, user_query):
        """
        Use OpenAI function calling to parse the user query and filter products.
        """
        try:
            # Create the system message with product context
            system_message = f"""You are a product search assistant. You have access to a dataset of {len(self.products)} products.
            
Available categories: Electronics, Fitness, Kitchen, Books, Clothing
Price range: $9.99 - $1299.99
Rating range: 4.0 - 4.8

When a user asks for products, analyze their request and call the filter_products function with appropriate criteria.
Be intelligent about interpreting natural language - for example:
- "under $300" means max_price: 300
- "at least 4 stars" means min_rating: 4.0
- "in stock" means in_stock_only: true
- "electronics" refers to category: "Electronics"
- "fitness tracker" might mean category: "Fitness" and keywords: ["tracker"]

Always call the filter_products function to process the user's request."""

            # Get the function schema
            function_schema = get_search_function_schema()
            
            # Make the API call with function calling
            response = self.client.chat.completions.create(
                model="gpt-4.1-mini",
                messages=[
                    {"role": "system", "content": system_message},
                    {"role": "user", "content": user_query}
                ],
                functions=[function_schema],
                function_call={"name": "filter_products"}
            )
            
            # Extract function call arguments
            function_call = response.choices[0].message.function_call
            if function_call and function_call.name == "filter_products":
                # Parse the arguments
                import json
                criteria = json.loads(function_call.arguments)
                
                # Apply the filtering
                filtered_products = filter_products_by_criteria(
                    self.products,
                    category=criteria.get('category'),
                    max_price=criteria.get('max_price'),
                    min_price=criteria.get('min_price'),
                    min_rating=criteria.get('min_rating'),
                    in_stock_only=criteria.get('in_stock_only'),
                    keywords=criteria.get('keywords')
                )
                
                return filtered_products, criteria
            else:
                print("❌ Error: OpenAI did not call the expected function.")
                return [], {}
                
        except Exception as e:
            print(f"❌ Error calling OpenAI API: {str(e)}")
            return [], {}
    
    def display_results(self, products, criteria):
        """Display the filtered products in a formatted way."""
        print("\n" + "="*60)
        print("🔍 SEARCH CRITERIA EXTRACTED BY AI:")
        print("="*60)
        
        for key, value in criteria.items():
            if value is not None:
                if key == 'max_price':
                    print(f"   • Maximum Price: ${value}")
                elif key == 'min_price':
                    print(f"   • Minimum Price: ${value}")
                elif key == 'min_rating':
                    print(f"   • Minimum Rating: {value} stars")
                elif key == 'in_stock_only' and value:
                    print(f"   • In Stock Only: Yes")
                elif key == 'category':
                    print(f"   • Category: {value}")
                elif key == 'keywords' and value:
                    print(f"   • Keywords: {', '.join(value)}")
        
        print("\n" + "="*60)
        print("🛍️  FILTERED PRODUCTS:")
        print("="*60)
        
        if not products:
            print("   No products found matching your criteria.")
            return
        
        for i, product in enumerate(products, 1):
            stock_status = "In Stock" if product.get('in_stock', False) else "Out of Stock"
            print(f"{i}. {product['name']} - ${product['price']}, Rating: {product['rating']}, {stock_status}")
        
        print(f"\nFound {len(products)} product(s) matching your criteria.")
    
    def run(self):
        """Main application loop."""
        print("🛍️  Welcome to the Natural Language Product Search Tool!")
        print("=" * 60)
        print("💡 Examples of what you can ask:")
        print("   • 'Show me in-stock electronics under $300 with at least 4 stars'")
        print("   • 'I want a fitness tracker with good battery life below $150'")
        print("   • 'Find me kitchen appliances over $50'")
        print("   • 'Show me books under $30'")
        print("=" * 60)
        
        while True:
            try:
                # Get user input
                user_query = input("\n🔍 What products are you looking for? (or 'quit' to exit): ").strip()
                
                if user_query.lower() in ['quit', 'exit', 'q']:
                    print("👋 Thanks for using the Product Search Tool!")
                    break
                
                if not user_query:
                    print("Please enter a search query.")
                    continue
                
                print("\n🤖 Processing your request with AI...")
                
                # Search products using OpenAI
                filtered_products, criteria = self.search_products(user_query)
                
                # Display results
                self.display_results(filtered_products, criteria)
                
            except KeyboardInterrupt:
                print("\n\n👋 Thanks for using the Product Search Tool!")
                break
            except Exception as e:
                print(f"❌ An error occurred: {str(e)}")

def main():
    """Entry point of the application."""
    app = ProductSearchTool()
    app.run()

if __name__ == "__main__":
    main()
