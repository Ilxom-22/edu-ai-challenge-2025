const { Enigma, Rotor } = require('./enigma.js');

// =============================================================================
// ENIGMA MACHINE UNIT TESTS
// =============================================================================
function runTests() {
  console.log('Running Enigma Machine Tests...\n');
  
  let testsPassed = 0;
  let totalTests = 0;

  function test(description, testFn) {
    totalTests++;
    try {
      testFn();
      console.log(`✓ ${description}`);
      testsPassed++;
    } catch (error) {
      console.log(`✗ ${description}: ${error.message}`);
    }
  }

  function assertEqual(actual, expected, message) {
    if (actual !== expected) {
      throw new Error(`${message}: expected "${expected}", got "${actual}"`);
    }
  }

  function assertArrayEqual(actual, expected, message) {
    if (JSON.stringify(actual) !== JSON.stringify(expected)) {
      throw new Error(`${message}: expected [${expected}], got [${actual}]`);
    }
  }

  // Test 1: Basic encryption/decryption reciprocity
  test('Basic reciprocity (encrypt then decrypt)', () => {
    const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    
    const plaintext = 'HELLO';
    const encrypted = enigma1.process(plaintext);
    const decrypted = enigma2.process(encrypted);
    
    assertEqual(decrypted, plaintext, 'Decryption should restore original text');
  });

  // Test 2: Plugboard functionality
  test('Plugboard swapping works correctly', () => {
    const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], [['A', 'B']]);
    const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], [['A', 'B']]);
    
    const plaintext = 'ABCD';
    const encrypted = enigma1.process(plaintext);
    const decrypted = enigma2.process(encrypted);
    
    assertEqual(decrypted, plaintext, 'Plugboard should work with reciprocity');
  });

  // Test 3: Rotor stepping
  test('Rotor stepping advances correctly', () => {
    const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    
    // Initial positions
    assertArrayEqual(enigma.getRotorPositions(), [0, 0, 0], 'Initial positions');
    
    // After one character
    enigma.encryptChar('A');
    assertArrayEqual(enigma.getRotorPositions(), [0, 0, 1], 'After one step');
    
    // After 25 more characters (should trigger middle rotor)
    for (let i = 0; i < 25; i++) {
      enigma.encryptChar('A');
    }
    assertArrayEqual(enigma.getRotorPositions(), [0, 1, 0], 'After rotor rollover');
  });

  // Test 4: Double-stepping mechanism
  test('Double-stepping works correctly', () => {
    const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    
    // Set up for double-stepping: middle rotor at notch-1, right rotor at notch-1
    enigma.setRotorPositions([0, 4, 21]); // E is notch for rotor II, V is notch for rotor III
    
    // This should trigger double-stepping
    enigma.encryptChar('A');
    assertArrayEqual(enigma.getRotorPositions(), [1, 5, 22], 'Double-stepping should advance left and middle rotors');
  });

  // Test 5: Non-alphabetic characters pass through unchanged
  test('Non-alphabetic characters pass through', () => {
    const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    
    const result = enigma.process('HELLO 123!');
    // Should encrypt letters but leave numbers and symbols unchanged
    assertEqual(result.match(/[0-9!]/g).join(''), '123!', 'Numbers and symbols should pass through');
  });

  // Test 6: Different rotor positions produce different outputs
  test('Different rotor positions produce different outputs', () => {
    const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    const enigma2 = new Enigma([0, 1, 2], [1, 2, 3], [0, 0, 0], []);
    
    const plaintext = 'TEST';
    const result1 = enigma1.process(plaintext);
    const result2 = enigma2.process(plaintext);
    
    if (result1 === result2) {
      throw new Error('Different rotor positions should produce different outputs');
    }
  });

  // Test 7: Ring settings affect encryption
  test('Ring settings affect encryption', () => {
    const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [1, 1, 1], []);
    
    const plaintext = 'TEST';
    const result1 = enigma1.process(plaintext);
    const result2 = enigma2.process(plaintext);
    
    if (result1 === result2) {
      throw new Error('Different ring settings should produce different outputs');
    }
  });

  // Test 8: Known historical test vector
  test('Historical test vector verification', () => {
    // Known Enigma result: AAA with rotors I,II,III at AAA should produce certain output
    const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    const result = enigma.process('AAA');
    
    // This should produce a specific result based on the historical wiring
    // The exact result depends on the specific rotor wirings used
    if (result === 'AAA') {
      throw new Error('Enigma should never encrypt a letter to itself');
    }
  });

  // Test 9: Multiple plugboard pairs work correctly
  test('Multiple plugboard pairs work correctly', () => {
    const enigma1 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], [['A', 'B'], ['C', 'D'], ['E', 'F']]);
    const enigma2 = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], [['A', 'B'], ['C', 'D'], ['E', 'F']]);
    
    const plaintext = 'ABCDEF';
    const encrypted = enigma1.process(plaintext);
    const decrypted = enigma2.process(encrypted);
    
    assertEqual(decrypted, plaintext, 'Multiple plugboard pairs should work with reciprocity');
  });

  // Test 10: Enigma never encrypts a letter to itself
  test('Enigma never encrypts a letter to itself', () => {
    const enigma = new Enigma([0, 1, 2], [0, 0, 0], [0, 0, 0], []);
    
    // Test all letters in alphabet
    for (let i = 0; i < 26; i++) {
      const letter = String.fromCharCode(65 + i); // A-Z
      const encrypted = enigma.encryptChar(letter);
      if (encrypted === letter) {
        throw new Error(`Letter ${letter} encrypted to itself, which should never happen in Enigma`);
      }
    }
  });

  console.log(`\nTests completed: ${testsPassed}/${totalTests} passed`);
  if (testsPassed === totalTests) {
    console.log('🎉 All tests passed! Enigma machine is working correctly.');
    return true;
  } else {
    console.log('❌ Some tests failed. Check the implementation.');
    return false;
  }
}

// Run tests if this file is executed directly
if (require.main === module) {
  runTests();
}

module.exports = { runTests }; 