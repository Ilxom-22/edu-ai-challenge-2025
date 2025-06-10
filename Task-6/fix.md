# Enigma Machine Bug Fixes

This document describes the bugs identified in the original Enigma machine implementation and the fixes that were applied to restore correct historical behavior.

## Summary

Two critical bugs were identified and fixed in the original `enigma.js` implementation:

1. **Missing Plugboard Swap (Critical)**
2. **Incorrect Double-Stepping Logic**

Both bugs prevented the implementation from behaving like the historical Enigma machine, affecting encryption accuracy and rotor advancement.

---

## Bug #1: Missing Plugboard Swap (Critical)

### Issue Description
The original code only applied plugboard swapping **once** at the beginning of the encryption process. However, the historical Enigma machine applies plugboard swapping **twice** - once before the signal enters the rotors and once after it returns from the reflector.

### Root Cause
In the original `encryptChar()` method:
```javascript
// ORIGINAL CODE (BUGGY)
encryptChar(c) {
  if (!alphabet.includes(c)) return c;
  this.stepRotors();
  
  c = plugboardSwap(c, this.plugboardPairs);  // ✓ First plugboard pass
  
  // Forward through rotors
  for (let i = this.rotors.length - 1; i >= 0; i--) {
    c = this.rotors[i].forward(c);
  }

  c = REFLECTOR[alphabet.indexOf(c)];

  // Backward through rotors
  for (let i = 0; i < this.rotors.length; i++) {
    c = this.rotors[i].backward(c);
  }

  // ❌ MISSING: Second plugboard pass
  return c;
}
```

### Impact
- **Plugboard pairs didn't work correctly in both directions**
- **Broke the reciprocal nature of Enigma encryption** (encrypt → decrypt wouldn't return original text)
- **Non-historical behavior** - real Enigma machines always applied plugboard twice

### Fix Applied
Added the missing second plugboard swap after the signal returns from the rotors:

```javascript
// FIXED CODE
encryptChar(c) {
  if (!alphabet.includes(c)) return c;
  this.stepRotors();
  
  // First plugboard pass
  c = plugboardSwap(c, this.plugboardPairs);
  
  // Forward through rotors (right to left)
  for (let i = this.rotors.length - 1; i >= 0; i--) {
    c = this.rotors[i].forward(c);
  }

  // Reflector
  c = REFLECTOR[alphabet.indexOf(c)];

  // Backward through rotors (left to right)
  for (let i = 0; i < this.rotors.length; i++) {
    c = this.rotors[i].backward(c);
  }

  // ✅ FIXED: Second plugboard pass
  c = plugboardSwap(c, this.plugboardPairs);
  return c;
}
```

### Verification
- **Reciprocity restored**: Encrypting the same text twice with identical settings returns the original
- **Plugboard pairs work correctly**: A↔B swapping works in both encryption and decryption
- **Historical accuracy**: Matches the behavior of real Enigma machines

---

## Bug #2: Incorrect Double-Stepping Logic

### Issue Description
The original `stepRotors()` method had flawed double-stepping logic that didn't match the historical Enigma machine's behavior.

### Root Cause
In the original code:
```javascript
// ORIGINAL CODE (BUGGY)
stepRotors() {
  if (this.rotors[2].atNotch()) this.rotors[1].step();  // Basic stepping
  if (this.rotors[1].atNotch()) this.rotors[0].step();  // Basic stepping
  this.rotors[2].step();  // Always step rightmost
}
```

**Problems:**
- Didn't implement the **double-stepping anomaly**
- Middle rotor would step again on the next keystroke if at notch position
- Didn't match the mechanical behavior of real Enigma machines

### Impact
- **Non-historical rotor advancement**
- **Incorrect encryption patterns** for sequences where double-stepping should occur
- **Failed to replicate authentic Enigma behavior**

### Fix Applied
Implemented the correct double-stepping mechanism:

```javascript
// FIXED CODE
stepRotors() {
  // Check if middle rotor is at notch position (will cause double-step)
  const middleAtNotch = this.rotors[1].atNotch();
  const rightAtNotch = this.rotors[2].atNotch();

  // Step left rotor if middle rotor at notch
  if (middleAtNotch) {
    this.rotors[0].step();
  }
  
  // Step middle rotor if it's at notch OR if right rotor at notch
  if (middleAtNotch || rightAtNotch) {
    this.rotors[1].step();
  }
  
  // Always step the right rotor
  this.rotors[2].step();
}
```

### Double-Stepping Explanation
In real Enigma machines, when the middle rotor reaches its notch position:
1. **Current keystroke**: Middle rotor steps (along with left rotor)
2. **Next keystroke**: Middle rotor steps again (due to mechanical linkage)

This creates the characteristic "double-step" where the middle rotor advances on consecutive keystrokes.

### Verification
- **Correct rotor advancement**: Matches historical Enigma I machine behavior
- **Double-stepping implemented**: Middle rotor correctly steps twice when at notch
- **Authentic encryption patterns**: Produces historically accurate results

---

## Conclusion

These fixes restore the Enigma machine implementation to **historically accurate behavior**. The simulation now correctly replicates the mechanical and electrical properties of the original WWII Enigma I machine, including:

- **Proper plugboard operation** (bidirectional swapping)
- **Authentic rotor stepping** (including double-stepping anomaly)
- **Reciprocal encryption** (encrypt twice = original text)
- **Historical constraints** (never encrypts letter to itself)

The implementation can now be used for educational purposes, historical simulation, or cryptographic research with confidence in its accuracy. 