"""
OpenAI Function Schemas for Product Search
"""

def get_search_function_schema():
    """
    Define the OpenAI function schema for product search and filtering.
    This function will be called by OpenAI to filter products based on user criteria.
    """
    return {
        "name": "filter_products",
        "description": "Filter products based on user criteria like category, price range, rating, stock status, and specific features",
        "parameters": {
            "type": "object",
            "properties": {
                "category": {
                    "type": "string",
                    "description": "Product category (e.g., Electronics, Fitness, Kitchen, Books, Clothing)"
                },
                "max_price": {
                    "type": "number",
                    "description": "Maximum price filter"
                },
                "min_price": {
                    "type": "number", 
                    "description": "Minimum price filter"
                },
                "min_rating": {
                    "type": "number",
                    "description": "Minimum rating filter (0-5)"
                },
                "in_stock_only": {
                    "type": "boolean",
                    "description": "Whether to show only in-stock items"
                },
                "keywords": {
                    "type": "array",
                    "items": {"type": "string"},
                    "description": "Keywords to search for in product names"
                }
            },
            "required": []
        }
    }

def filter_products_by_criteria(products, category=None, max_price=None, min_price=None, 
                               min_rating=None, in_stock_only=None, keywords=None):
    """
    Filter products based on the provided criteria.
    This function is called after OpenAI determines the filtering criteria.
    """
    filtered_products = products.copy()
    
    # Apply category filter
    if category:
        filtered_products = [p for p in filtered_products 
                           if p.get('category', '').lower() == category.lower()]
    
    # Apply price filters
    if max_price is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('price', 0) <= max_price]
    
    if min_price is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('price', 0) >= min_price]
    
    # Apply rating filter
    if min_rating is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('rating', 0) >= min_rating]
    
    # Apply stock filter
    if in_stock_only:
        filtered_products = [p for p in filtered_products 
                           if p.get('in_stock', False)]
    
    # Apply keyword filter
    if keywords:
        keyword_filtered = []
        for product in filtered_products:
            product_name = product.get('name', '').lower()
            if any(keyword.lower() in product_name for keyword in keywords):
                keyword_filtered.append(product)
        filtered_products = keyword_filtered
    
    return filtered_products 