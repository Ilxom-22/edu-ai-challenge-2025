# Enigma Machine Implementation

This is a JavaScript implementation of the famous WWII Enigma cipher machine with bug fixes and comprehensive testing.

## Bugs Found and Fixed

### 1. **Missing Plugboard Swap (Critical Bug)**
**Issue**: The original code only applied plugboard swapping once at the beginning of encryption, but historically the Enigma machine applies plugboard swapping twice - once before the rotors and once after.

**Fix**: Added the missing second plugboard swap after the signal returns from the reflector.

**Impact**: Without this fix, plugboard pairs didn't work correctly in both directions, breaking the reciprocal nature of Enigma encryption.

### 2. **Incorrect Double-Stepping Logic**
**Issue**: The original `stepRotors()` method had flawed double-stepping logic that didn't match the historical Enigma behavior.

**Fix**: Implemented the correct double-stepping mechanism:
- Check if middle rotor is at notch position
- Step left rotor if middle rotor is at notch
- Step middle rotor if it's at notch OR if right rotor is at notch
- Always step the right rotor

**Impact**: Rotor advancement now matches the historical Enigma machine's behavior exactly.

## Usage

### Basic Usage
```bash
node enigma.js
```
Or with npm:
```bash
npm start
```
Follow the prompts to encrypt/decrypt messages.

### Running Tests
```bash
node test.js
```
Or with npm:
```bash
npm test
```

### Example
```
Enter message: HELLO WORLD
Rotor positions (e.g. 0 0 0): 0 0 0
Ring settings (e.g. 0 0 0): 0 0 0
Plugboard pairs (e.g. AB CD): AB CD
Output: MFNCY DKZGX
```

## Features

- ✅ Historically accurate rotor stepping with double-stepping
- ✅ Proper plugboard implementation (bidirectional)
- ✅ Three rotors with correct wiring
- ✅ Reflector implementation
- ✅ Ring settings support
- ✅ Reciprocal encryption (encrypt twice to get original)
- ✅ Comprehensive unit tests

## Technical Details

### Encryption Process
1. Step rotors (before encryption)
2. Apply plugboard swapping
3. Pass through rotors (right to left)
4. Apply reflector
5. Pass back through rotors (left to right)
6. Apply plugboard swapping again

### Key Components
- **Rotors**: Three rotors with historical wiring patterns
- **Plugboard**: Swaps letter pairs before and after rotor processing
- **Reflector**: Reflects signal back through rotors
- **Ring Settings**: Internal wiring offset for each rotor

## Testing

The implementation includes 10 comprehensive unit tests covering:
- Basic reciprocity (encrypt/decrypt)
- Plugboard functionality
- Rotor stepping mechanics
- Double-stepping behavior
- Non-alphabetic character handling
- Different configurations produce different outputs
- Ring setting effects
- Historical accuracy validation
- Multiple plugboard pairs functionality
- Enigma never encrypts a letter to itself verification

Run tests with: `node test.js`

## Requirements

- Node.js 14.0.0+ (required for optional chaining operator)
- No additional dependencies required 