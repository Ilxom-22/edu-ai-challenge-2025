# Sample Application Outputs

This document shows real execution examples of the Natural Language Product Search Tool.

## 🎯 Sample Run #1: Electronics Search

### Input Query:
```
🔍 What products are you looking for? Show me in-stock electronics under $300 with at least 4 stars
```

### Output:
```
🤖 Processing your request with AI...

============================================================
🔍 SEARCH CRITERIA EXTRACTED BY AI:
============================================================
   • Category: Electronics
   • Maximum Price: $300
   • Minimum Rating: 4.0 stars
   • In Stock Only: Yes

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
1. Wireless Headphones - $99.99, Rating: 4.5, In Stock
2. Smart Watch - $199.99, Rating: 4.6, In Stock
3. Bluetooth Speaker - $49.99, Rating: 4.4, In Stock
4. 4K Monitor - $349.99, Rating: 4.7, In Stock
5. Noise-Cancelling Headphones - $299.99, Rating: 4.8, In Stock
6. Gaming Mouse - $59.99, Rating: 4.3, In Stock
7. External Hard Drive - $89.99, Rating: 4.4, In Stock
8. Portable Charger - $29.99, Rating: 4.2, In Stock

Found 8 product(s) matching your criteria.
```

---

## 🏋️ Sample Run #2: Fitness Equipment Search

### Input Query:
```
🔍 What products are you looking for? I want fitness equipment below $100 with good ratings
```

### Output:
```
🤖 Processing your request with AI...

============================================================
🔍 SEARCH CRITERIA EXTRACTED BY AI:
============================================================
   • Category: Fitness
   • Maximum Price: $100
   • Minimum Rating: 4.0 stars

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
1. Yoga Mat - $19.99, Rating: 4.3, In Stock
2. Resistance Bands - $14.99, Rating: 4.1, In Stock
3. Kettlebell - $39.99, Rating: 4.3, In Stock
4. Foam Roller - $24.99, Rating: 4.5, In Stock
5. Pull-up Bar - $59.99, Rating: 4.4, In Stock
6. Ab Roller - $19.99, Rating: 4.2, In Stock

Found 6 product(s) matching your criteria.
```

---

## 📖 Sample Run #3: Books with Keyword Search

### Input Query:
```
🔍 What products are you looking for? Find me programming or science books under $50
```

### Output:
```
🤖 Processing your request with AI...

============================================================
🔍 SEARCH CRITERIA EXTRACTED BY AI:
============================================================
   • Category: Books
   • Maximum Price: $50
   • Keywords: programming, science

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
1. Programming Guide - $49.99, Rating: 4.7, In Stock
2. History of Science - $39.99, Rating: 4.6, In Stock

Found 2 product(s) matching your criteria.
```

---

## 🔍 Sample Run #4: Complex Multi-Criteria Search

### Input Query:
```
🔍 What products are you looking for? Show me kitchen appliances that are in stock, cost between $50 and $200, and have at least 4.5 stars
```

### Output:
```
🤖 Processing your request with AI...

============================================================
🔍 SEARCH CRITERIA EXTRACTED BY AI:
============================================================
   • Category: Kitchen
   • Minimum Price: $50
   • Maximum Price: $200
   • Minimum Rating: 4.5 stars
   • In Stock Only: Yes

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
1. Air Fryer - $89.99, Rating: 4.6, In Stock
2. Microwave Oven - $129.99, Rating: 4.5, In Stock
3. Pressure Cooker - $99.99, Rating: 4.7, In Stock

Found 3 product(s) matching your criteria.
```

---

## 🚫 Sample Run #5: No Results Found

### Input Query:
```
🔍 What products are you looking for? I want electronics under $20 with 5-star ratings
```

### Output:
```
🤖 Processing your request with AI...

============================================================
🔍 SEARCH CRITERIA EXTRACTED BY AI:
============================================================
   • Category: Electronics
   • Maximum Price: $20
   • Minimum Rating: 5.0 stars

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
   No products found matching your criteria.

Found 0 product(s) matching your criteria.
```

---

## 📊 Analysis of Sample Runs

### Key Observations:

1. **AI Understanding**: The AI successfully interprets various natural language patterns:
   - "under $300" → max_price: 300
   - "at least 4 stars" → min_rating: 4.0
   - "in-stock" → in_stock_only: true
   - "between $50 and $200" → min_price: 50, max_price: 200

2. **Flexible Queries**: The system handles:
   - Single criteria (category only)
   - Multiple criteria combinations
   - Keyword-based searches
   - Complex price ranges

3. **Clear Output**: Results show both:
   - AI-extracted criteria for transparency
   - Formatted product list with essential details

4. **Edge Cases**: Gracefully handles scenarios with no matching products

5. **Natural Language Variations**: Successfully processes different phrasings:
   - "Show me..." vs "I want..." vs "Find me..."
   - "good ratings" vs "at least 4 stars"
   - "below $100" vs "under $100"

These examples demonstrate the tool's ability to understand natural language and convert it into structured search criteria using OpenAI's function calling capability. 