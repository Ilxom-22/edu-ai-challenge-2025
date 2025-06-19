# Sample Application Outputs

This document shows real execution examples of the Natural Language Product Search Tool.

## 🎯 Sample Run #1: Electronics Search

### Input Query:
```
🔍 What products are you looking for? (or 'quit' to exit): Show me in-stock electronics under $300 with at least 4 stars
```

### Output:
```
🤖 Processing your request with AI...

============================================================
🔍 SEARCH CRITERIA EXTRACTED BY AI:
============================================================
   • Category: Electronics
   • Maximum Price: $300
   • Minimum Rating: 4 stars
   • In Stock Only: Yes

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
1. Wireless Headphones - $99.99, Rating: 4.5, In Stock
2. Smart Watch - $199.99, Rating: 4.6, In Stock
3. Bluetooth Speaker - $49.99, Rating: 4.4, In Stock
4. Noise-Cancelling Headphones - $299.99, Rating: 4.8, In Stock
5. Gaming Mouse - $59.99, Rating: 4.3, In Stock
6. External Hard Drive - $89.99, Rating: 4.4, In Stock
7. Portable Charger - $29.99, Rating: 4.2, In Stock

Found 7 product(s) matching your criteria.
```

---

## 🏋️ Sample Run #2: Fitness Equipment Search

### Input Query:
```
🔍 What products are you looking for? (or 'quit' to exit): I want fitness equipment below $100 with good ratings
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
6. Jump Rope - $9.99, Rating: 4.0, In Stock
7. Ab Roller - $19.99, Rating: 4.2, In Stock

Found 7 product(s) matching your criteria.
```

---

## 📖 Sample Run #3: Books with Keyword Search

### Input Query:
```
🔍 What products are you looking for? (or 'quit' to exit): Find me programming or science books under $50
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
3. Science Fiction Novel - $17.99, Rating: 4.2, In Stock

Found 3 product(s) matching your criteria.
```

---

## 🔍 Sample Run #4: Complex Multi-Criteria Search

### Input Query:
```
🔍 What products are you looking for? (or 'quit' to exit): Show me kitchen appliances that are in stock, cost between $50 and $200, and have at least 4.5 stars
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
2. Pressure Cooker - $99.99, Rating: 4.7, In Stock

Found 2 product(s) matching your criteria.
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
   • Minimum Rating: 5 stars
   • Maximum Rating: 5 stars

============================================================
🛍️  FILTERED PRODUCTS:
============================================================
   No products found matching your criteria.
```

