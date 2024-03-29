using System;
using System.Threading;

class TicTacToe3D {

    private int[,,] board;
    private int player; // 1: Player 1, 2: Player 2 (CPU)
    private int mask = (1 << 4) - 1;

    public TicTacToe3D() {
        board = new int[4, 4, 4];
        player = 1; // Start with player 1
    }

    public bool PlacePiece(int x, int y, int z) {
        if (board[x, y, z] == 0) {
            board[x, y, z] = player;
            //Console.WriteLine("Player {0} placed a piece on {1}, {2}, {3}", player, x, y, z);
            player = (player == 1) ? 2 : 1; // switch sides
            return true;
        }
        //Console.WriteLine("Unable to place player ${0} piece on {1}, {2}, {3}", player, x, y, z);
        return false;
    }

    public bool PlacePiece(int move) {
        return PlacePiece(move >> 8, (move >> 4) & mask, move & mask);
    }

    public int CheckWinner() {
        // Check all cells for winning situation (this is an overkill, but no problem, board is small
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                for (int k = 0; k < 4; k++) {
                    if (IsWinner(i, j, k)) { return board[i, j, k]; }
                }
            }
        }
        // Check for tie
        if (IsBoardFull()) { return 0; }
        // No winner yet
        return -1;
    }

    private bool IsWinner(int x, int y, int z) {
        int value = board[x, y, z];
        if (value == 0) { return false; }
        // Check all directions in 3 dimensions
        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                for (int dz = -1; dz <= 1; dz++) {
                    if (dx == 0 && dy == 0 && dz == 0) { continue; } // Skip the current cell
                    int count = 0;
                    for (int i = -3; i < 4; i++) {
                        int nx = x + i * dx;
                        int ny = y + i * dy;
                        int nz = z + i * dz;
                        if (nx >= 0 && nx < 4 &&
                            ny >= 0 && ny < 4 &&
                            nz >= 0 && nz < 4 &&
                            board[nx, ny, nz] == value) { count++; }
                        else { break; }
                    }
                    if (count >= 4) { return true; }
                }
            }
        }
        return false;
    }

    private bool IsBoardFull() {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                for (int k = 0; k < 4; k++) {
                    if (board[i, j, k] == 0) { return false; }
                }
            }
        }
        return true;
    }

    public int CPUMove() {
        // Think a bit
        Thread.Sleep(50);
        // Prioritize winning moves (offensive)
        int move = OffensiveMove();
        if (move != -1) { return move; }
        // Block opponent's winning moves (defensive)
        move = DefensiveMove();
        if (move != -1) { return move; }
        // Choose a random empty cell (fallback)
        return RandomMove();
    }

    private int OffensiveMove() {
        // Check all empty cells for potential winning moves
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 4; y++) {
                for (int z = 0; z < 4; z++) {
                    if (board[x, y, z] == 0) {
                        // Simulate placing the CPU piece at this cell
                        board[x, y, z] = player;
                        if (CheckWinner() == player) {
                            // Found a winning move for CPU, return it
                            board[x, y, z] = 0; // Revert the simulation
                            return (x << 8) | (y << 4) | z; // Encode the move as a single integer
                        }
                        board[x, y, z] = 0; // Revert the simulation
                    }
                }
            }
        }
        return -1; // No winning move found
    }

    private int DefensiveMove() {
        // Check all empty cells for potential opponent's winning moves
        for (int x = 0; x < 4; x++) {
            for (int y = 0; y < 4; y++) {
                for (int z = 0; z < 4; z++) {
                    if (board[x, y, z] == 0) {
                        // Simulate placing opponent's piece at this cell
                        int oponnent = (player == 1) ? 2 : 1;
                        board[x, y, z] = oponnent; // Opponent's piece
                        if (CheckWinner() == oponnent) { // Check winner for opponent
                            // Found a potential losing move for CPU, block it
                            board[x, y, z] = 0; // Revert the simulation
                            return (x << 8) | (y << 4) | z; // Encode the move as a single integer
                        }
                        board[x, y, z] = 0; // Revert the simulation
                    }
                }
            }
        }
        return -1; // No opponent's winning move found
    }

    private Random random = new Random();

    private int RandomMove() {
        // Choose a random empty cell
        while (true) {
            int x = random.Next(4);
            int y = random.Next(4);
            int z = random.Next(4);
            if (board[x, y, z] == 0) {
                return (x << 8) | (y << 4) | z; // Encode the move as a single integer
            }
        }
    }

    public void DrawBoard() {
        const char O = '\u25ef';
        const char X = '\u2613';
        const char vtdiv = '\u2502';
        const char hzdiv = '\u2500';
        const char cross = '\u253c';
        string hline = string.Format("{0}{1}{0}{1}{0}{1}{0}", hzdiv, cross);
        int i = 0;
        do {
            int j = 0;
            do {
               int k = 0;
               do {
                   int piece = board[i,j,k];
                   Console.Write((piece == 1) ? O : ((piece == 2) ? X : ' '));
                   if (k++ > 2) { break; }
                   Console.Write(vtdiv);
               } while(true);
               if (j++ > 2) { break; }
               Console.Write("  ");
            } while(true);
            Console.WriteLine();
            if (i++ > 2) { break; }
            Console.WriteLine("{0}  {0}  {0}  {0}", hline);
        } while(true);
    }

    static void Main(string[] args) {
        // Test - put CPU to play with itself
        TicTacToe3D game = new TicTacToe3D();
        int winner;
        Console.WriteLine();
        game.DrawBoard();
        do {
            int move = game.CPUMove();
            if (!game.PlacePiece(move)) { // placing piece also switches player
                Console.WriteLine("SOMETHING WENT WRONG, COULD NOT PLACE PIECE");
                break;
            }
            Console.WriteLine("\x1b[8F"); // go back 8 lines
            game.DrawBoard();
            winner = game.CheckWinner();
        } while (winner == -1);
        Console.WriteLine();
        switch(winner) {
            case 0: Console.WriteLine("It's a tie!"); break;
            case 1:
            case 2: Console.WriteLine("Player {0} wins!", winner); break;
            default: Console.WriteLine("ERROR! SHOULD NOT HAPPEN, GAME STILL GOING."); break;
        }
    }

}
