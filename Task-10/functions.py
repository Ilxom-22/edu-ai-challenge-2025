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
        "description": "Filter and sort products based on user criteria like category, price range, rating, stock status, specific features, and sorting preferences",
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
                "price_range": {
                    "type": "object",
                    "properties": {
                        "min": {"type": "number", "description": "Minimum price"},
                        "max": {"type": "number", "description": "Maximum price"}
                    },
                    "description": "Price range filter. Use this for queries like 'between $50 and $200'. Alternative to using separate min_price and max_price."
                },
                "min_rating": {
                    "type": "number",
                    "description": "Minimum rating filter (0-5)"
                },
                "max_rating": {
                    "type": "number",
                    "description": "Maximum rating filter (0-5). Use this for queries like 'rating lower than X' or 'rating below X'"
                },
                "in_stock_only": {
                    "type": "boolean",
                    "description": "Filter by stock status. Set to true for 'in stock' items, false for 'out of stock' items, or null/undefined to include both"
                },
                "keywords": {
                    "type": "array",
                    "items": {"type": "string"},
                    "description": "Keywords to search for in product names (case-insensitive, partial matching). Use this for searches like 'laptop', 'bluetooth', 'fitness tracker', etc."
                },
                "sort_by": {
                    "type": "string",
                    "enum": ["price_asc", "price_desc", "rating_asc", "rating_desc", "name_asc", "name_desc"],
                    "description": "Sort products by: 'price_asc' (cheapest first), 'price_desc' (most expensive first), 'rating_asc' (worst rating first), 'rating_desc' (best rating first), 'name_asc' (A-Z), 'name_desc' (Z-A). Use 'price_asc' for 'cheapest', 'price_desc' for 'most expensive', 'rating_desc' for 'best rating', 'rating_asc' for 'worst rating'"
                },
                "limit": {
                    "type": "integer",
                    "description": "Maximum number of products to return. Use this when user asks for 'top 5', 'first 10', etc. Default is no limit."
                }
            },
            "required": []
        }
    }

def filter_products_by_criteria(products, category=None, max_price=None, min_price=None, 
                               price_range=None, min_rating=None, max_rating=None, in_stock_only=None, keywords=None,
                               sort_by=None, limit=None):
    """
    Filter and sort products based on the provided criteria.
    This function is called after OpenAI determines the filtering criteria.
    """
    filtered_products = products.copy()
    
    # Apply category filter
    if category:
        filtered_products = [p for p in filtered_products 
                           if p.get('category', '').lower() == category.lower()]
    
    # Handle price_range parameter (takes precedence over individual min/max price)
    if price_range:
        try:
            if isinstance(price_range, dict):
                if price_range.get('min') is not None:
                    min_price = price_range['min']
                if price_range.get('max') is not None:
                    max_price = price_range['max']
            elif isinstance(price_range, str):
                # If price_range comes as a string, try to parse it
                import json
                try:
                    parsed_range = json.loads(price_range)
                    if parsed_range.get('min') is not None:
                        min_price = parsed_range['min']
                    if parsed_range.get('max') is not None:
                        max_price = parsed_range['max']
                except json.JSONDecodeError:
                    pass  # Ignore if can't parse
        except Exception:
            pass  # Ignore price_range if there's any issue
    
    # Apply price filters
    if max_price is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('price', 0) <= max_price]
    
    if min_price is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('price', 0) >= min_price]
    
    # Apply rating filters
    if min_rating is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('rating', 0) >= min_rating]
    
    if max_rating is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('rating', 0) <= max_rating]
    
    # Apply stock filter
    if in_stock_only is not None:
        filtered_products = [p for p in filtered_products 
                           if p.get('in_stock', False) == in_stock_only]
    
    # Apply keyword filter
    if keywords:
        keyword_filtered = []
        for product in filtered_products:
            product_name = product.get('name', '').lower()
            if any(keyword.lower() in product_name for keyword in keywords):
                keyword_filtered.append(product)
        filtered_products = keyword_filtered
    
    # Apply sorting
    if sort_by:
        if sort_by == "price_asc":
            filtered_products.sort(key=lambda x: x.get('price', 0))
        elif sort_by == "price_desc":
            filtered_products.sort(key=lambda x: x.get('price', 0), reverse=True)
        elif sort_by == "rating_asc":
            filtered_products.sort(key=lambda x: x.get('rating', 0))
        elif sort_by == "rating_desc":
            filtered_products.sort(key=lambda x: x.get('rating', 0), reverse=True)
        elif sort_by == "name_asc":
            filtered_products.sort(key=lambda x: x.get('name', '').lower())
        elif sort_by == "name_desc":
            filtered_products.sort(key=lambda x: x.get('name', '').lower(), reverse=True)
    
    # Apply limit
    if limit is not None and limit > 0:
        filtered_products = filtered_products[:limit]
    
    return filtered_products 