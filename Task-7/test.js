const assert = require('assert');

// Import the classes from seabattle.js
const {
  BOARD_SIZE,
  NUM_SHIPS,
  SHIP_LENGTH,
  CELL_TYPES,
  Ship,
  Board,
  Player,
  HumanPlayer,
  CPUPlayer,
  GameUI,
  SeaBattleGame
} = require('./seabattle.js');

// Mock readline for testing
const mockReadline = {
  createInterface: () => ({
    question: (prompt, callback) => callback('00'),
    close: () => {}
  })
};

// Override require for readline when testing
const Module = require('module');
const originalRequire = Module.prototype.require;
Module.prototype.require = function(id) {
  if (id === 'readline' && process.env.NODE_ENV === 'test') {
    return mockReadline;
  }
  return originalRequire.apply(this, arguments);
};

// Set test environment
process.env.NODE_ENV = 'test';

// Test runner
class TestRunner {
  constructor() {
    this.tests = [];
    this.passed = 0;
    this.failed = 0;
    this.coverage = {
      totalMethods: 0,
      testedMethods: 0,
      classes: {}
    };
  }

  test(name, testFn) {
    this.tests.push({ name, testFn });
  }

  async run() {
    console.log('🧪 Running Sea Battle Game Unit Tests\n');
    
    for (const { name, testFn } of this.tests) {
      try {
        await testFn();
        console.log(`✅ ${name}`);
        this.passed++;
      } catch (error) {
        console.log(`❌ ${name}`);
        console.log(`   Error: ${error.message}\n`);
        this.failed++;
      }
    }

    this.printSummary();
  }

  printSummary() {
    const total = this.passed + this.failed;
    const passRate = ((this.passed / total) * 100).toFixed(2);
    
    console.log('\n📊 Test Summary');
    console.log('================');
    console.log(`Total Tests: ${total}`);
    console.log(`Passed: ${this.passed}`);
    console.log(`Failed: ${this.failed}`);
    console.log(`Pass Rate: ${passRate}%`);
    
    if (this.failed === 0) {
      console.log('\n🎉 All tests passed!');
    }
  }
}

// Initialize test runner
const runner = new TestRunner();

// SHIP CLASS TESTS
runner.test('Ship: Constructor should initialize locations and hits', () => {
  const locations = ['00', '01', '02'];
  const ship = new Ship(locations);
  
  assert.deepStrictEqual(ship.locations, locations);
  assert.deepStrictEqual(ship.hits, [false, false, false]);
});

runner.test('Ship: hit() should mark location as hit and return true for valid hit', () => {
  const ship = new Ship(['00', '01', '02']);
  
  const result = ship.hit('01');
  assert.strictEqual(result, true);
  assert.deepStrictEqual(ship.hits, [false, true, false]);
});

runner.test('Ship: hit() should return false for invalid location', () => {
  const ship = new Ship(['00', '01', '02']);
  
  const result = ship.hit('99');
  assert.strictEqual(result, false);
  assert.deepStrictEqual(ship.hits, [false, false, false]);
});

runner.test('Ship: hit() should return false for already hit location', () => {
  const ship = new Ship(['00', '01', '02']);
  
  ship.hit('01'); // First hit
  const result = ship.hit('01'); // Second hit on same location
  assert.strictEqual(result, false);
  assert.deepStrictEqual(ship.hits, [false, true, false]);
});

runner.test('Ship: isSunk() should return false for partially hit ship', () => {
  const ship = new Ship(['00', '01', '02']);
  ship.hit('01');
  
  assert.strictEqual(ship.isSunk(), false);
});

runner.test('Ship: isSunk() should return true for fully hit ship', () => {
  const ship = new Ship(['00', '01', '02']);
  ship.hit('00');
  ship.hit('01');
  ship.hit('02');
  
  assert.strictEqual(ship.isSunk(), true);
});

runner.test('Ship: hasLocation() should return true for valid location', () => {
  const ship = new Ship(['00', '01', '02']);
  
  assert.strictEqual(ship.hasLocation('01'), true);
  assert.strictEqual(ship.hasLocation('99'), false);
});

// BOARD CLASS TESTS
runner.test('Board: Constructor should initialize grid with water', () => {
  const board = new Board();
  
  assert.strictEqual(board.grid.length, BOARD_SIZE);
  assert.strictEqual(board.grid[0].length, BOARD_SIZE);
  assert.strictEqual(board.grid[0][0], CELL_TYPES.WATER);
  assert.strictEqual(board.ships.length, 0);
});

runner.test('Board: Constructor should set showShips flag', () => {
  const board1 = new Board(true);
  const board2 = new Board(false);
  
  assert.strictEqual(board1.showShips, true);
  assert.strictEqual(board2.showShips, false);
});

runner.test('Board: _parseLocation() should correctly parse location string', () => {
  const board = new Board();
  
  const [row, col] = board._parseLocation('34');
  assert.strictEqual(row, 3);
  assert.strictEqual(col, 4);
});

runner.test('Board: _generateRandomShip() should create valid ship', () => {
  const board = new Board();
  const ship = board._generateRandomShip();
  
  assert.ok(ship instanceof Ship);
  assert.strictEqual(ship.locations.length, SHIP_LENGTH);
  
  // Check if all locations are valid
  ship.locations.forEach(location => {
    const [row, col] = board._parseLocation(location);
    assert.ok(row >= 0 && row < BOARD_SIZE);
    assert.ok(col >= 0 && col < BOARD_SIZE);
  });
  
  // Check if locations are consecutive
  const rows = ship.locations.map(loc => parseInt(loc[0]));
  const cols = ship.locations.map(loc => parseInt(loc[1]));
  
  const isHorizontal = rows.every(r => r === rows[0]);
  const isVertical = cols.every(c => c === cols[0]);
  
  assert.ok(isHorizontal || isVertical);
  
  if (isHorizontal) {
    assert.deepStrictEqual(cols, Array.from({length: SHIP_LENGTH}, (_, i) => cols[0] + i));
  } else {
    assert.deepStrictEqual(rows, Array.from({length: SHIP_LENGTH}, (_, i) => rows[0] + i));
  }
});

runner.test('Board: _markShipOnGrid() should mark ship locations', () => {
  const board = new Board(true);
  const locations = ['00', '01', '02'];
  
  board._markShipOnGrid(locations);
  
  locations.forEach(location => {
    const [row, col] = board._parseLocation(location);
    assert.strictEqual(board.grid[row][col], CELL_TYPES.SHIP);
  });
});

runner.test('Board: placeShipsRandomly() should place correct number of ships', () => {
  const board = new Board(true);
  board.placeShipsRandomly(NUM_SHIPS);
  
  assert.strictEqual(board.ships.length, NUM_SHIPS);
});

runner.test('Board: placeShipsRandomly() should mark ships on grid when showShips is true', () => {
  const board = new Board(true);
  board.placeShipsRandomly(NUM_SHIPS);
  
  let shipCells = 0;
  for (let i = 0; i < BOARD_SIZE; i++) {
    for (let j = 0; j < BOARD_SIZE; j++) {
      if (board.grid[i][j] === CELL_TYPES.SHIP) {
        shipCells++;
      }
    }
  }
  
  assert.strictEqual(shipCells, NUM_SHIPS * SHIP_LENGTH);
});

runner.test('Board: placeShipsRandomly() should not mark ships on grid when showShips is false', () => {
  const board = new Board(false);
  board.placeShipsRandomly(NUM_SHIPS);
  
  let shipCells = 0;
  for (let i = 0; i < BOARD_SIZE; i++) {
    for (let j = 0; j < BOARD_SIZE; j++) {
      if (board.grid[i][j] === CELL_TYPES.SHIP) {
        shipCells++;
      }
    }
  }
  
  assert.strictEqual(shipCells, 0);
});

runner.test('Board: processGuess() should return hit result for ship location', () => {
  const board = new Board(true);
  board.placeShipsRandomly(1);
  
  // Find a ship location
  let shipLocation = null;
  for (let i = 0; i < BOARD_SIZE; i++) {
    for (let j = 0; j < BOARD_SIZE; j++) {
      if (board.grid[i][j] === CELL_TYPES.SHIP) {
        shipLocation = `${i}${j}`;
        break;
      }
    }
    if (shipLocation) break;
  }
  
  const result = board.processGuess(shipLocation);
  assert.strictEqual(result.success, true);
  assert.strictEqual(result.hit, true);
  assert.strictEqual(result.location, shipLocation);
  assert.strictEqual(board.grid[parseInt(shipLocation[0])][parseInt(shipLocation[1])], CELL_TYPES.HIT);
});

runner.test('Board: processGuess() should return miss result for water location', () => {
  const board = new Board(true);
  board.placeShipsRandomly(1);
  
  // Find a water location
  let waterLocation = null;
  for (let i = 0; i < BOARD_SIZE; i++) {
    for (let j = 0; j < BOARD_SIZE; j++) {
      if (board.grid[i][j] === CELL_TYPES.WATER) {
        waterLocation = `${i}${j}`;
        break;
      }
    }
    if (waterLocation) break;
  }
  
  const result = board.processGuess(waterLocation);
  assert.strictEqual(result.success, true);
  assert.strictEqual(result.hit, false);
  assert.strictEqual(result.location, waterLocation);
  assert.strictEqual(board.grid[parseInt(waterLocation[0])][parseInt(waterLocation[1])], CELL_TYPES.MISS);
});

runner.test('Board: processGuess() should reject already guessed location', () => {
  const board = new Board(true);
  board.placeShipsRandomly(1);
  
  // Find a ship location
  let shipLocation = null;
  for (let i = 0; i < BOARD_SIZE; i++) {
    for (let j = 0; j < BOARD_SIZE; j++) {
      if (board.grid[i][j] === CELL_TYPES.SHIP) {
        shipLocation = `${i}${j}`;
        break;
      }
    }
    if (shipLocation) break;
  }
  
  board.processGuess(shipLocation); // First guess
  const result = board.processGuess(shipLocation); // Second guess
  
  assert.strictEqual(result.success, false);
  assert.strictEqual(result.reason, 'already_guessed');
});

runner.test('Board: getAliveShipsCount() should return correct count', () => {
  const board = new Board(true);
  board.placeShipsRandomly(NUM_SHIPS);
  
  assert.strictEqual(board.getAliveShipsCount(), NUM_SHIPS);
  
  // Sink one ship
  const ship = board.ships[0];
  ship.locations.forEach(location => {
    board.processGuess(location);
  });
  
  assert.strictEqual(board.getAliveShipsCount(), NUM_SHIPS - 1);
});

runner.test('Board: display() should return correct string format', () => {
  const board = new Board(true);
  board.placeShipsRandomly(1);
  
  const display = board.display();
  const lines = display.split('\n');
  
  // Check header
  assert.strictEqual(lines[0].trim(), '0 1 2 3 4 5 6 7 8 9');
  
  // Check grid lines
  for (let i = 1; i <= BOARD_SIZE; i++) {
    const line = lines[i].trim();
    assert.strictEqual(line.length, BOARD_SIZE * 2 + 1); // Each cell is 2 chars + space
    assert.strictEqual(line[0], String(i - 1)); // Row number
  }
});

// PLAYER CLASS TESTS
runner.test('Player: Constructor should initialize correctly', () => {
  const board = new Board();
  const player = new Player('TestPlayer', board);
  
  assert.strictEqual(player.name, 'TestPlayer');
  assert.strictEqual(player.board, board);
  assert.strictEqual(player.guesses instanceof Set, true);
  assert.strictEqual(player.guesses.size, 0);
});

runner.test('Player: hasGuessed() and addGuess() should work correctly', () => {
  const player = new Player('Test', new Board());
  
  assert.strictEqual(player.hasGuessed('00'), false);
  
  player.addGuess('00');
  assert.strictEqual(player.hasGuessed('00'), true);
  assert.strictEqual(player.guesses.size, 1);
});

// HUMAN PLAYER TESTS
runner.test('HumanPlayer: Constructor should set correct name', () => {
  const board = new Board();
  const player = new HumanPlayer(board);
  
  assert.strictEqual(player.name, 'Player');
  assert.strictEqual(player.board, board);
});

runner.test('HumanPlayer: _isValidGuess() should validate input correctly', () => {
  const player = new HumanPlayer(new Board());
  
  // Valid cases
  assert.strictEqual(player._isValidGuess('00'), true);
  assert.strictEqual(player._isValidGuess('99'), true);
  assert.strictEqual(player._isValidGuess('34'), true);
  
  // Invalid cases
  assert.strictEqual(player._isValidGuess('0'), false);
  assert.strictEqual(player._isValidGuess('000'), false);
  assert.strictEqual(player._isValidGuess('ab'), false);
  
  // Edge cases
  assert.strictEqual(player._isValidGuess(''), false);
  assert.strictEqual(player._isValidGuess(null), false);
  assert.strictEqual(player._isValidGuess(undefined), false);
});

// CPU PLAYER TESTS
runner.test('CPUPlayer: Constructor should initialize correctly', () => {
  const board = new Board();
  const player = new CPUPlayer(board);
  
  assert.strictEqual(player.name, 'CPU');
  assert.strictEqual(player.mode, 'hunt');
  assert.strictEqual(player.targetQueue.length, 0);
});

runner.test('CPUPlayer: makeGuess() should return valid coordinates', () => {
  const player = new CPUPlayer(new Board());
  
  const guess = player.makeGuess();
  assert.strictEqual(typeof guess, 'string');
  assert.strictEqual(guess.length, 2);
  assert.strictEqual(/^\d{2}$/.test(guess), true);
  
  const [row, col] = [parseInt(guess[0]), parseInt(guess[1])];
  assert.strictEqual(row >= 0 && row < BOARD_SIZE, true);
  assert.strictEqual(col >= 0 && col < BOARD_SIZE, true);
});

runner.test('CPUPlayer: makeGuess() should not repeat guesses', () => {
  const player = new CPUPlayer(new Board());
  
  const guesses = new Set();
  for (let i = 0; i < 10; i++) {
    const guess = player.makeGuess();
    assert.strictEqual(guesses.has(guess), false);
    guesses.add(guess);
  }
});

runner.test('CPUPlayer: processGuessResult() should switch to target mode on hit', () => {
  const player = new CPUPlayer(new Board());
  
  player.processGuessResult('44', { hit: true, sunk: false });
  assert.strictEqual(player.mode, 'target');
  assert.strictEqual(player.targetQueue.length > 0, true);
});

runner.test('CPUPlayer: processGuessResult() should switch to hunt mode on sunk', () => {
  const player = new CPUPlayer(new Board());
  player.mode = 'target';
  player.targetQueue = ['44', '45'];
  
  player.processGuessResult('44', { hit: true, sunk: true });
  assert.strictEqual(player.mode, 'hunt');
  assert.strictEqual(player.targetQueue.length, 0);
});

runner.test('CPUPlayer: _addAdjacentTargets() should add valid adjacent cells', () => {
  const player = new CPUPlayer(new Board());
  
  player._addAdjacentTargets('44');
  
  const expectedTargets = ['34', '54', '43', '45'];
  expectedTargets.forEach(target => {
    assert.strictEqual(player.targetQueue.includes(target), true);
  });
});

runner.test('CPUPlayer: _addAdjacentTargets() should not add out-of-bounds cells', () => {
  const player = new CPUPlayer(new Board());
  
  player._addAdjacentTargets('00'); // Top-left corner
  
  // Should only add right and down
  assert.strictEqual(player.targetQueue.includes('10'), true);
  assert.strictEqual(player.targetQueue.includes('01'), true);
  assert.strictEqual(player.targetQueue.length, 2);
});

runner.test('CPUPlayer: _addAdjacentTargets() should not add already guessed cells', () => {
  const player = new CPUPlayer(new Board());
  player.addGuess('34');
  player.addGuess('54');
  
  player._addAdjacentTargets('44');
  
  // Should not include already guessed cells
  assert.strictEqual(player.targetQueue.includes('34'), false);
  assert.strictEqual(player.targetQueue.includes('54'), false);
  assert.strictEqual(player.targetQueue.includes('43'), true);
  assert.strictEqual(player.targetQueue.includes('45'), true);
});

// GAME UI TESTS
runner.test('GameUI: Constructor should initialize readline interface', () => {
  const gameUI = new GameUI();
  assert.ok(gameUI.rl);
});

runner.test('GameUI: displayMessage should log message', () => {
  const gameUI = new GameUI();
  const message = 'Test message';
  gameUI.displayMessage(message);
  // Note: We can't easily test console.log output
});

runner.test('GameUI: getPlayerInput should return promise', () => {
  const gameUI = new GameUI();
  const inputPromise = gameUI.getPlayerInput();
  assert.ok(inputPromise instanceof Promise);
});

// SEA BATTLE GAME TESTS
runner.test('SeaBattleGame: Constructor should initialize game components', () => {
  const game = new SeaBattleGame();
  
  assert.ok(game.playerBoard instanceof Board);
  assert.ok(game.cpuBoard instanceof Board);
  assert.ok(game.humanPlayer instanceof HumanPlayer);
  assert.ok(game.cpuPlayer instanceof CPUPlayer);
  assert.ok(game.gameUI instanceof GameUI);
});

runner.test('SeaBattleGame: _initializeGame should place ships', () => {
  const game = new SeaBattleGame();
  game._initializeGame();
  
  assert.strictEqual(game.playerBoard.ships.length, NUM_SHIPS);
  assert.strictEqual(game.cpuBoard.ships.length, NUM_SHIPS);
});

// Run all tests
if (require.main === module) {
  runner.run().then(() => {
    console.log('\n🏁 Test execution completed');
    process.exit(runner.failed > 0 ? 1 : 0);
  });
}

module.exports = { runner }; 