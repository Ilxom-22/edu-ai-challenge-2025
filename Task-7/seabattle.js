const readline = require('readline');

// Constants
const BOARD_SIZE = 10;
const NUM_SHIPS = 3;
const SHIP_LENGTH = 3;
const CELL_TYPES = {
  WATER: '~',
  SHIP: 'S',
  HIT: 'X',
  MISS: 'O'
};

// Ship class to represent individual ships
class Ship {
  constructor(locations) {
    this.locations = locations;
    this.hits = new Array(locations.length).fill(false);
  }

  hit(location) {
    const index = this.locations.indexOf(location);
    if (index >= 0 && !this.hits[index]) {
      this.hits[index] = true;
      return true;
    }
    return false;
  }

  isSunk() {
    return this.hits.every(hit => hit);
  }

  hasLocation(location) {
    return this.locations.includes(location);
  }
}

// Board class to manage the game board
class Board {
  constructor(showShips = false) {
    this.grid = Array(BOARD_SIZE).fill().map(() => Array(BOARD_SIZE).fill(CELL_TYPES.WATER));
    this.ships = [];
    this.showShips = showShips;
  }

  placeShipsRandomly(numShips) {
    let placedShips = 0;
    
    while (placedShips < numShips) {
      const ship = this._generateRandomShip();
      if (ship && this._canPlaceShip(ship.locations)) {
        this.ships.push(ship);
        if (this.showShips) {
          this._markShipOnGrid(ship.locations);
        }
        placedShips++;
      }
    }
  }

  _generateRandomShip() {
    const orientation = Math.random() < 0.5 ? 'horizontal' : 'vertical';
    let startRow, startCol;

    if (orientation === 'horizontal') {
      startRow = Math.floor(Math.random() * BOARD_SIZE);
      startCol = Math.floor(Math.random() * (BOARD_SIZE - SHIP_LENGTH + 1));
    } else {
      startRow = Math.floor(Math.random() * (BOARD_SIZE - SHIP_LENGTH + 1));
      startCol = Math.floor(Math.random() * BOARD_SIZE);
    }

    const locations = [];
    for (let i = 0; i < SHIP_LENGTH; i++) {
      const row = orientation === 'horizontal' ? startRow : startRow + i;
      const col = orientation === 'horizontal' ? startCol + i : startCol;
      locations.push(`${row}${col}`);
    }

    return new Ship(locations);
  }

  _canPlaceShip(locations) {
    return locations.every(location => {
      const [row, col] = this._parseLocation(location);
      return row >= 0 && row < BOARD_SIZE && 
             col >= 0 && col < BOARD_SIZE && 
             this.grid[row][col] === CELL_TYPES.WATER;
    });
  }

  _markShipOnGrid(locations) {
    locations.forEach(location => {
      const [row, col] = this._parseLocation(location);
      this.grid[row][col] = CELL_TYPES.SHIP;
    });
  }

  processGuess(guess) {
    const [row, col] = this._parseLocation(guess);
    
    if (this.grid[row][col] === CELL_TYPES.HIT || this.grid[row][col] === CELL_TYPES.MISS) {
      return { success: false, reason: 'already_guessed' };
    }

    let hitShip = null;
    for (const ship of this.ships) {
      if (ship.hit(guess)) {
        hitShip = ship;
        break;
      }
    }

    if (hitShip) {
      this.grid[row][col] = CELL_TYPES.HIT;
      return {
        success: true,
        hit: true,
        sunk: hitShip.isSunk(),
        location: guess
      };
    } else {
      this.grid[row][col] = CELL_TYPES.MISS;
      return {
        success: true,
        hit: false,
        location: guess
      };
    }
  }

  _parseLocation(location) {
    return [parseInt(location[0]), parseInt(location[1])];
  }

  getAliveShipsCount() {
    return this.ships.filter(ship => !ship.isSunk()).length;
  }

  display() {
    let output = '  ';
    for (let i = 0; i < BOARD_SIZE; i++) {
      output += `${i} `;
    }
    output += '\n';

    for (let i = 0; i < BOARD_SIZE; i++) {
      output += `${i} `;
      for (let j = 0; j < BOARD_SIZE; j++) {
        output += `${this.grid[i][j]} `;
      }
      output += '\n';
    }
    return output;
  }
}

// Base Player class
class Player {
  constructor(name, board) {
    this.name = name;
    this.board = board;
    this.guesses = new Set();
  }

  hasGuessed(location) {
    return this.guesses.has(location);
  }

  addGuess(location) {
    this.guesses.add(location);
  }
}

// Human Player class
class HumanPlayer extends Player {
  constructor(board) {
    super('Player', board);
  }

  async makeGuess(gameUI) {
    while (true) {
      try {
        const guess = await gameUI.getPlayerInput();
        
        if (!this._isValidGuess(guess)) {
          gameUI.displayMessage('Oops, input must be exactly two digits (e.g., 00, 34, 98).');
          continue;
        }

        const [row, col] = [parseInt(guess[0]), parseInt(guess[1])];
        if (row < 0 || row >= BOARD_SIZE || col < 0 || col >= BOARD_SIZE) {
          gameUI.displayMessage(`Oops, please enter valid row and column numbers between 0 and ${BOARD_SIZE - 1}.`);
          continue;
        }

        if (this.hasGuessed(guess)) {
          gameUI.displayMessage('You already guessed that location!');
          continue;
        }

        this.addGuess(guess);
        return guess;
      } catch (error) {
        gameUI.displayMessage('Invalid input. Please try again.');
      }
    }
  }

  _isValidGuess(guess) {
    return !!(guess && guess.length === 2 && /^\d{2}$/.test(guess));
  }
}

// CPU Player class with AI
class CPUPlayer extends Player {
  constructor(board) {
    super('CPU', board);
    this.mode = 'hunt';
    this.targetQueue = [];
  }

  makeGuess() {
    let guess;

    if (this.mode === 'target' && this.targetQueue.length > 0) {
      guess = this.targetQueue.shift();
      if (this.hasGuessed(guess)) {
        if (this.targetQueue.length === 0) {
          this.mode = 'hunt';
        }
        return this.makeGuess(); // Recursive call to try again
      }
    } else {
      this.mode = 'hunt';
      do {
        const row = Math.floor(Math.random() * BOARD_SIZE);
        const col = Math.floor(Math.random() * BOARD_SIZE);
        guess = `${row}${col}`;
      } while (this.hasGuessed(guess));
    }

    this.addGuess(guess);
    return guess;
  }

  processGuessResult(guess, result) {
    if (result.hit) {
      if (result.sunk) {
        this.mode = 'hunt';
        this.targetQueue = [];
      } else {
        this.mode = 'target';
        this._addAdjacentTargets(guess);
      }
    } else if (this.mode === 'target' && this.targetQueue.length === 0) {
      this.mode = 'hunt';
    }
  }

  _addAdjacentTargets(location) {
    const [row, col] = [parseInt(location[0]), parseInt(location[1])];
    const adjacent = [
      { r: row - 1, c: col },
      { r: row + 1, c: col },
      { r: row, c: col - 1 },
      { r: row, c: col + 1 }
    ];

    adjacent.forEach(({ r, c }) => {
      if (r >= 0 && r < BOARD_SIZE && c >= 0 && c < BOARD_SIZE) {
        const adjLocation = `${r}${c}`;
        if (!this.hasGuessed(adjLocation) && !this.targetQueue.includes(adjLocation)) {
          this.targetQueue.push(adjLocation);
        }
      }
    });
  }
}

// Game UI class for display and input handling
class GameUI {
  constructor() {
    this.rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout
    });
  }

  displayBoards(playerBoard, opponentBoard) {
    console.log('\n   --- OPPONENT BOARD ---          --- YOUR BOARD ---');
    
    const header = '  ' + Array.from({length: BOARD_SIZE}, (_, i) => i).join(' ');
    console.log(header + '     ' + header);

    for (let i = 0; i < BOARD_SIZE; i++) {
      let rowStr = `${i} `;
      
      // Opponent board (don't show ships)
      for (let j = 0; j < BOARD_SIZE; j++) {
        const cell = opponentBoard.grid[i][j];
        rowStr += (cell === CELL_TYPES.SHIP ? CELL_TYPES.WATER : cell) + ' ';
      }
      
      rowStr += `    ${i} `;
      
      // Player board (show ships)
      for (let j = 0; j < BOARD_SIZE; j++) {
        rowStr += playerBoard.grid[i][j] + ' ';
      }
      
      console.log(rowStr);
    }
    console.log();
  }

  displayMessage(message) {
    console.log(message);
  }

  async getPlayerInput() {
    return new Promise((resolve) => {
      this.rl.question('Enter your guess (e.g., 00): ', (answer) => {
        resolve(answer);
      });
    });
  }

  close() {
    this.rl.close();
  }
}

// Main Game class
class SeaBattleGame {
  constructor() {
    this.playerBoard = new Board(true);
    this.cpuBoard = new Board(false);
    this.humanPlayer = new HumanPlayer(this.playerBoard);
    this.cpuPlayer = new CPUPlayer(this.cpuBoard);
    this.gameUI = new GameUI();
  }

  async start() {
    this._initializeGame();
    await this._gameLoop();
  }

  _initializeGame() {
    console.log('Creating boards...');
    this.playerBoard.placeShipsRandomly(NUM_SHIPS);
    this.cpuBoard.placeShipsRandomly(NUM_SHIPS);
    
    console.log(`\nLet's play Sea Battle!`);
    console.log(`Try to sink the ${NUM_SHIPS} enemy ships.`);
  }

  async _gameLoop() {
    while (true) {
      // Check win conditions
      if (this.cpuBoard.getAliveShipsCount() === 0) {
        this.gameUI.displayMessage('\n*** CONGRATULATIONS! You sunk all enemy battleships! ***');
        this.gameUI.displayBoards(this.playerBoard, this.cpuBoard);
        break;
      }
      
      if (this.playerBoard.getAliveShipsCount() === 0) {
        this.gameUI.displayMessage('\n*** GAME OVER! The CPU sunk all your battleships! ***');
        this.gameUI.displayBoards(this.playerBoard, this.cpuBoard);
        break;
      }

      // Display current state
      this.gameUI.displayBoards(this.playerBoard, this.cpuBoard);

      // Player turn
      const playerGuess = await this.humanPlayer.makeGuess(this.gameUI);
      const playerResult = this.cpuBoard.processGuess(playerGuess);
      
      if (playerResult.hit) {
        this.gameUI.displayMessage('PLAYER HIT!');
        if (playerResult.sunk) {
          this.gameUI.displayMessage('You sunk an enemy battleship!');
        }
      } else {
        this.gameUI.displayMessage('PLAYER MISS.');
      }

      // Check if player won
      if (this.cpuBoard.getAliveShipsCount() === 0) continue;

      // CPU turn
      this.gameUI.displayMessage("\n--- CPU's Turn ---");
      const cpuGuess = this.cpuPlayer.makeGuess();
      const cpuResult = this.playerBoard.processGuess(cpuGuess);
      
      if (cpuResult.hit) {
        this.gameUI.displayMessage(`CPU HIT at ${cpuGuess}!`);
        if (cpuResult.sunk) {
          this.gameUI.displayMessage('CPU sunk your battleship!');
        }
      } else {
        this.gameUI.displayMessage(`CPU MISS at ${cpuGuess}.`);
      }

      this.cpuPlayer.processGuessResult(cpuGuess, cpuResult);
    }

    this.gameUI.close();
  }
}

// Start the game
const game = new SeaBattleGame();
game.start().catch(console.error);