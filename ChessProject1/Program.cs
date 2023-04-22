using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            
            ChessGame game = new ChessGame();
            game.startNewChessGame(game);

        }
        class ChessGame
        {
            ChessPiece[,] board;
            string moveRequest;
            bool isBlackTurn;
            int movesWithoutCapture;
            int currentBoardPieces;
            int turnNumber;          
            string[] boardHistory = new string[50];
            public ChessPiece[,] getBoard() { return this.board; }
            public ChessGame()
            {
                this.moveRequest = "";
                this.isBlackTurn = false;
                this.movesWithoutCapture = 0;
                this.currentBoardPieces = 32;
                this.boardHistory = new string[50];
                this.turnNumber = 1;
                this.board = new ChessPiece[8, 8] {{ new Rook(true), new Knight(true), new Bishop(true), new Queen(true), new King(true), new Bishop(true),new Knight(true), new Rook(true) },
                                                { new Pawn(true),  new Pawn(true),  new Pawn(true),  new Pawn(true), new Pawn(true), new Pawn(true), new Pawn(true),  new Pawn(true) },
                                                {  null,  null, null, null, null,  null,  null, null},
                                                {  null,  null, null, null, null,  null,  null, null},
                                                {  null,  null, null, null, null,  null,  null, null},
                                                {  null,  null, null, null, null,  null,  null, null},
                                                { new Pawn(false),  new Pawn(false),  new Pawn(false),  new Pawn(false), new Pawn(false), new Pawn(false), new Pawn(false),  new Pawn(false) },
                                                { new Rook(false), new Knight(false), new Bishop(false), new Queen(false), new King(false), new Bishop(false),new Knight(false), new Rook(false) } };

            }
            public void startNewChessGame(ChessGame currentGame) // starts the game
            {
                int finishType = 0;
                bool gameOver = false;
                
                PrintChessBoard(board);
                while (!gameOver)
                {
                    Console.WriteLine((isBlackTurn ? "black turn " : " white turn ") + " please enter a move request and press enter ");
                    string input = Console.ReadLine();
                    if (!CheckInputFromUser(input)) { continue; }
                    input=changeToIndex(input);
                    Point startPoint = changeToPoint(input, 1, 0);
                    Point endpPoint = changeToPoint(input, 3, 2);
                    if (board[startPoint.getRow(), startPoint.getColumn()] == null)
                        continue;
                    if (board[startPoint.getRow(), startPoint.getColumn()].getColor() == isBlackTurn)
                    {
                        if (board[startPoint.getRow(), startPoint.getColumn()].isLegalMove(currentGame,isBlackTurn, startPoint, endpPoint, board))
                        {
                            if (board[startPoint.getRow(), startPoint.getColumn()] is Pawn)
                                movesWithoutCapture = 0;
                            board[startPoint.getRow(), startPoint.getColumn()].Move(currentGame,isBlackTurn,startPoint, endpPoint, board);
                            PrintChessBoard(board);
                            isBlackTurn = !isBlackTurn;                           
                            turnNumber++;                           
                        }
                        else
                            Console.WriteLine(" this is illegal move ");
                    }
                    if (isCheckmate(currentGame, isBlackTurn, board) == true || isDrawByStalment(currentGame, board, isBlackTurn) || isDrawByFiftyMove() || isDrawByDeadPosition(board) || isDrawByThreefoldRepetition(board))
                    {
                        gameOver = true;
                        Console.WriteLine("GAME OVER");
                    }
                }
               
               
            }
            public ChessPiece saveMoveForCheck(ChessPiece[,] board, Point startpoint, Point endpoint)
            {
                ChessPiece tmppiece = board[endpoint.getRow(), endpoint.getColumn()];
                board[endpoint.getRow(), endpoint.getColumn()] = board[startpoint.getRow(), startpoint.getColumn()];
                board[startpoint.getRow(), startpoint.getColumn()] = null;
                return tmppiece;

            }
            public void undoMove(ChessPiece[,] board, Point startPoint, Point endPoint, ChessPiece piece)
            {
                board[startPoint.getRow(), startPoint.getColumn()] = board[endPoint.getRow(), endPoint.getColumn()];
                board[endPoint.getRow(), endPoint.getColumn()] = piece;
            }
            public void PrintChessBoard(ChessPiece[,] board)
            {
                Console.WriteLine("   A  B  C  D  E  F  G  H ");
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    Console.Write(8 - i + " ");
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] == null)
                            Console.Write("() ");
                        else
                            Console.Write(board[i, j].toString()+" ");
                    }
                    Console.Write(8 - i + " ");
                    Console.WriteLine();
                }
                Console.WriteLine("   A  B  C  D  E  F  G  H ");
            }//print board game 
            public Point getKingPlace(ChessPiece[,] board, bool isBlack)
            {
                Point place = new Point(0, 0);
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] != null && board[i, j] is King && board[i, j].getColor() == isBlack) 
                        {                         
                          place.setRow(i);
                          place.setColumn(j);
                           return place;                           
                        }
                    }
                }
                return place;
            }
            public bool isCheck(ChessGame currentGame,bool isBlack, Point kingPlace, ChessPiece[,] board)
            {

                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        Point tmppoint = new Point(i, j);
                        if (board[i, j] != null && board[i, j].getColor() == isBlack)
                        {
                            if (board[i, j].isLegalMove(currentGame, isBlack, tmppoint, kingPlace, board))
                                return true;                           
                        }
                    }
                }
                return false;
            }
            public bool isCheckmate( ChessGame currentGame,bool isBlack, ChessPiece[,] board)
            {
                Point startpoint = new Point(0, 0);
                Point endpoint = new Point(0, 0);
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] != null && board[i, j].getColor() == isBlack)
                        {
                            startpoint.setRow(i);
                            startpoint.setColumn(j);
                            for (int row = 0; row < board.GetLength(0); row++)
                            {
                                for (int coulmn = 0; coulmn < board.GetLength(1); coulmn++)
                                {
                                    if (i != row || j != coulmn)
                                    {
                                        endpoint.setRow(row);
                                        endpoint.setColumn(coulmn);
                                        if (board[i, j].isLegalMove(currentGame,isBlack, startpoint, endpoint, board))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            public ChessPiece coronation(ChessPiece pieceToUpdate)
            {
                string choice;
                Console.WriteLine("Your pawn has reached coronation status, please choose what type you want to change \n1 ) pawn \n 2) rook \n 3) knight \n 4) bishop \n 5) queen \n ");              
                bool ifChoose = false;
                while (ifChoose == false)
                {
                    choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":ifChoose = true;                            
                            break;
                        case "2": pieceToUpdate = new Rook(pieceToUpdate.getColor());ifChoose = true;                                                      
                            break;
                        case "3": pieceToUpdate = new Knight(pieceToUpdate.getColor());ifChoose = true;                     
                            break;
                        case "4": pieceToUpdate = new Bishop(pieceToUpdate.getColor());ifChoose = true;                   
                            break;
                        case "5": pieceToUpdate = new Queen(pieceToUpdate.getColor());ifChoose = true;                    
                            break;
                        default:  Console.WriteLine("please enter correct choice 1-5 ");                    
                            break;
                    }
                }
                return pieceToUpdate;
            }
            public int getTurnNumber() { return this.turnNumber; }
            public bool CheckInputFromUser(string input)
            {
                input.Trim();
                if (input.Length != 4)                
                    return false;           
                for (int i = 0; i < 3; i += 2)
                {
                    switch (input[i])
                    {
                        case 'h':case 'H':break;                                                
                        case 'g':case 'G':break;                                                 
                        case 'f':case 'F':break;                                                  
                        case 'e':case 'E':break;                                                 
                        case 'd':case 'D':break ;                                                  
                        case 'c':case 'C':break;                                                    
                        case 'b':case 'B':break;                                                    
                        case 'a':case 'A':break;                                                  
                        default:
                            return false;
                    }
                }
                for (int i = 1; i <= 3; i += 2)
                {
                    switch (input[i])
                    {
                        case '1':break;                           
                        case '2':break;                        
                        case '3':break;                       
                        case '4':break;                            
                        case '5':break;                          
                        case '6':break;                          
                        case '7':break;
                        case '8':break;                           
                        default:
                            return false;
                    }
                }
                return true; ;
            }//return true if the input correct
            public string changeToIndex(string input)
            {               
                string str = "";
                for (int i = 0; i < 3; i += 2)
                {
                    switch (input[i])
                    {
                        case 'h':case 'H':                       
                            str += "7" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;                          
                        case 'g':case 'G':                       
                            str += "6" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;
                        case 'f':case 'F':
                            str += "5" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;
                        case 'e':case 'E':
                            str += "4" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;
                        case 'd':case 'D':
                            str += "3" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;
                        case 'c':case 'C':
                            str += "2" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;
                        case 'b':case 'B':
                            str += "1" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;
                        case 'a':case 'A':
                            str += "0" + int.Parse((8 - int.Parse(input[i + 1].ToString())).ToString().ToString()) + "";break;                         
                    }
                }
                return str;
            }
            public bool isDrawByDeadPosition(ChessPiece[,] board)
            {
                if (this.currentBoardPieces == 2)                
                    return true;
                
                else if (this.currentBoardPieces == 3)
                {
                    for (int i = 0; i < board.GetLength(0); i++)
                    {
                        for (int j = 0; j < board.GetLength(1); j++)
                        {
                            if (board[i, j] != null)
                            {
                                if (board[i, j] is Bishop)
                                    return true;

                            }
                        }
                    }
                }
                return false;
            }
            public bool isDrawByThreefoldRepetition(ChessPiece[,] board)
            {
                string currentBoard = "";
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] == null)
                        {
                            currentBoard += "null";
                            continue;
                        }
                        currentBoard += (board[i, j]).ToString();
                        if (board[i, j] is Pawn || board[i, j] is King || board[i, j] is Rook)
                            currentBoard += board[i, j].getNeverMoved().ToString();
                    }
                }
                boardHistory[turnNumber] = currentBoard;
                for (int i = 0; i < this.turnNumber; i++)
                {
                    int repetition = 0;
                    for (int j = 0; j < this.turnNumber; j++)
                    {
                        if (boardHistory[j] != null)
                            if (boardHistory[i] == boardHistory[j])
                                repetition++;
                        if (repetition == 3)
                            return true;
                    }
                }
                return false;
            }
            public bool isDrawByFiftyMove() 
            {
                if (this.movesWithoutCapture == 50) { return true; }
                int boardPieces = 0;
                for (int row = 0; row < 8; row++) 
                {
                    for (int column = 0; column < 8; column++)
                    {
                        if (this.board[row, column] ==null)
                            continue;
                        else
                            boardPieces++;
                    }
                }
                if (this.currentBoardPieces != boardPieces) 
                {
                    this.movesWithoutCapture = 0;
                    this.currentBoardPieces = boardPieces;
                }
                else
                    this.movesWithoutCapture++;
                return false;
            }
            public bool isDrawByStalment(ChessGame currentGame,ChessPiece[,] board, bool isBlack)
            {
                if (this.isCheck(currentGame,!isBlack, getKingPlace(board, isBlack), board))
                    return false;

                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if (board[i, j] != null && board[i, j].getColor() == isBlack)
                        {
                            startPoint.setRow(i);
                            startPoint.setColumn(j);
                            for (int row = 0; row < board.GetLength(0); row++)
                            {
                                for (int coulmn = 0; coulmn < board.GetLength(1); coulmn++)
                                {
                                    if (i != row || j != coulmn)
                                    {
                                        endPoint.setRow(row);
                                        endPoint.setColumn(coulmn);
                                        if (board[i, j].isLegalMove(currentGame,isBlack, startPoint, endPoint, board))
                                        {

                                            return false;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            public void gameOverMessage(bool isBlack, int finishType)
            {
                switch (finishType)
                {
                    case 1:
                        Console.WriteLine("It's checkmate! Congratulations, {0} win.\n", isBlack ? "white" : "black");
                        break;
                    case 2:
                        Console.WriteLine("It's a draw! -> draw by dead position.\n");
                        break;
                    case 3:
                        Console.WriteLine("It's draw! -> draw by stalemate.\n");
                        break;
                    case 4:
                        Console.WriteLine("It's a draw! -> draw by threefold repetition.\n");
                        break;
                    case 5:
                        Console.WriteLine("It's a draw! -> draw by fifty moves.\n");
                        break;
                    default:
                        break;
                }
            }
            public Point changeToPoint(string moveIndexes, int row, int coulmn)
            {

                Point p = new Point(int.Parse(moveIndexes[row].ToString()), int.Parse(moveIndexes[coulmn].ToString()));
                return p;

            }
        }
        class Point
        {
            int row;
            int coulmn;
            public Point(int row, int column)
            {
                setRow(row);
                setColumn(column);
            }
            public void setRow(int row)
            {
                this.row = row;
            }
            public void setColumn(int column)
            {
                this.coulmn = column;
            }
            public int getRow()
            {
                return this.row;
            }
            public int getColumn()
            {
                return this.coulmn;
            }
        }
        class Pawn : ChessPiece
        {
            int doublestep;
            public Pawn(bool isblack) : base(isblack) { }
            public int getDoubleStep(){return this.doublestep;}
            public void setDoubleStep(int doubleStepTurn) { this.doublestep = doubleStepTurn; }           
            public override bool isLegalMove(ChessGame currentGame, bool color, Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                bool isLegal = false;
                bool enpassant = false;
                if (color == true)
                {
                    if (startPoint.getRow() == 1)
                    {
                        if ((endPoint.getRow() == startPoint.getRow() + 2 && startPoint.getColumn() == endPoint.getColumn()) && (board[startPoint.getRow() + 1, startPoint.getColumn()] == null && board[endPoint.getRow(), endPoint.getColumn()] == null))//מהלך ראשון של צעד כפול
                        {
                            isLegal = true;
                        }
                    }
                    if (endPoint.getRow() == startPoint.getRow() + 1)//מהלך רגיל צעד קדימה של פיון שחור
                    {
                        if ((endPoint.getColumn() == startPoint.getColumn() - 1) && board[endPoint.getRow(), endPoint.getColumn()] != null)//הכאה ימינה של פיון שחור מכה  לבן
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()].getColor() == false)
                                isLegal = true;
                        }
                        else if ((endPoint.getColumn() == startPoint.getColumn() - 1) && board[endPoint.getRow(), endPoint.getColumn()] == null)
                        {
                            if (board[endPoint.getRow() - 1, endPoint.getColumn()] != null)
                            {
                                if (board[endPoint.getRow() - 1, endPoint.getColumn()].getColor() == false && board[endPoint.getRow() - 1, endPoint.getColumn()] is Pawn && startPoint.getRow() == 4)
                                {
                                    if (((Pawn)board[endPoint.getRow() - 1, endPoint.getColumn()]).getDoubleStep() ==currentGame.getTurnNumber() - 1)
                                    {
                                        isLegal = true;
                                        enpassant = true;
                                    }
                                }
                            }
                        }
                        if ((endPoint.getColumn() == startPoint.getColumn() + 1) && board[endPoint.getRow(), endPoint.getColumn()] != null)//הכאה שמאלה של פיון שחור מכה לבן
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()].getColor() == false)
                                isLegal = true;
                        }
                        else if ((endPoint.getColumn() == startPoint.getColumn() + 1) && board[endPoint.getRow(), endPoint.getColumn()] == null)
                        {
                            if (board[endPoint.getRow() - 1, endPoint.getColumn()] != null)
                            {
                                if (board[endPoint.getRow() - 1, endPoint.getColumn()].getColor() == false && board[endPoint.getRow() - 1, endPoint.getColumn()] is Pawn && startPoint.getRow() == 4)
                                {
                                    if (((Pawn)board[endPoint.getRow() - 1, endPoint.getColumn()]).getDoubleStep() == currentGame.getTurnNumber() - 1)
                                    {
                                        isLegal = true;
                                        enpassant = true;
                                    }
                                }
                            }
                        }
                        if ((endPoint.getColumn() == startPoint.getColumn()) && board[endPoint.getRow(), endPoint.getColumn()] == null)//צעד קדימה של פיון שחור
                        {
                            isLegal = true;
                        }
                    }
                }
                if (color == false)
                {
                    if (startPoint.getRow() == 6)
                    {
                        if ((endPoint.getRow() == startPoint.getRow() - 2 && startPoint.getColumn() == endPoint.getColumn()) && (board[startPoint.getRow() - 1, startPoint.getColumn()] == null && board[endPoint.getRow(), endPoint.getColumn()] == null))//מהלך ראשון של צעד כפול
                        {
                            isLegal = true;
                        }
                    }
                    if (endPoint.getRow() == startPoint.getRow() - 1)//מהלך רגיל צעד קדימה של פיון לבן
                    {
                        if ((endPoint.getColumn() == startPoint.getColumn() + 1) && board[endPoint.getRow(), endPoint.getColumn()] != null)//הכאה ימינה של פיון לבן מכה פיון שחור
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()].getColor() == true)
                                isLegal = true;
                        }
                        else if ((endPoint.getColumn() == startPoint.getColumn() + 1) && board[endPoint.getRow(), endPoint.getColumn()] == null)
                        {
                            if (board[endPoint.getRow() + 1, endPoint.getColumn()] != null)
                            {
                                if (board[endPoint.getRow() + 1, endPoint.getColumn()].getColor() == true && board[endPoint.getRow() + 1, endPoint.getColumn()] is Pawn && startPoint.getRow() == 3)
                                {
                                    if (((Pawn)board[endPoint.getRow() + 1, endPoint.getColumn()]).getDoubleStep() == currentGame.getTurnNumber() - 1)
                                    {
                                        isLegal = true;
                                        enpassant = true;
                                    }
                                }
                            }
                        }
                        if ((endPoint.getColumn() == startPoint.getColumn() - 1) && board[endPoint.getRow(), endPoint.getColumn()] != null)//הכאה שמאלה של פיון לבן מכה פיון שחור
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()].getColor() == true)
                                isLegal = true;
                        }
                        else if ((endPoint.getColumn() == startPoint.getColumn() - 1) && board[endPoint.getRow(), endPoint.getColumn()] == null)
                        {
                            if (board[endPoint.getRow() + 1, endPoint.getColumn()] != null)
                            {
                                if (board[endPoint.getRow() + 1, endPoint.getColumn()].getColor() == true && board[endPoint.getRow() + 1, endPoint.getColumn()] is Pawn && startPoint.getRow() == 3)
                                {
                                    if (((Pawn)board[endPoint.getRow() + 1, endPoint.getColumn()]).getDoubleStep() == currentGame.getTurnNumber() - 1)
                                    {
                                        isLegal = true;
                                        enpassant = true;
                                    }
                                }
                            }
                        }
                        if ((endPoint.getColumn() == startPoint.getColumn()) && board[endPoint.getRow(), endPoint.getColumn()] == null)//צעד קדימה של פיון לבן
                        {
                            isLegal = true;
                        }
                    }
                }
                if (isLegal == true)
                {
                    ChessPiece temporaryPiece =  currentGame.saveMoveForCheck(board, startPoint, endPoint);
                    ChessPiece temporaryPieceForEnPassant = null;
                    if (enpassant == true)
                    {
                        temporaryPieceForEnPassant = board[startPoint.getRow(), endPoint.getColumn()];
                        Point tmppoint = new Point(startPoint.getRow(), endPoint.getColumn());
                        temporaryPieceForEnPassant = currentGame.saveMoveForCheck(board, startPoint, tmppoint);
                        if (currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board))                        
                            isLegal = false;                      
                        currentGame.undoMove(board, startPoint, tmppoint, temporaryPieceForEnPassant);
                        currentGame.undoMove(board, startPoint, endPoint, temporaryPiece);
                    }
                    else
                    {
                        if (currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board))                        
                            isLegal = false;                       
                        currentGame.undoMove(board, startPoint, endPoint, temporaryPiece);
                    }
                }
                return isLegal;
            }
            public override void Move(ChessGame currentGame, bool isBlackTurn, Point startpoint, Point endpoint, ChessPiece[,] board)
            {
                board[startpoint.getRow(), startpoint.getColumn()].setNeverMoved(false);
                if (startpoint.getRow() == endpoint.getRow() - 2 || startpoint.getRow() == endpoint.getRow() + 2)
                {
                    if (board[startpoint.getRow(), startpoint.getColumn()] is Pawn)                    
                        ((Pawn)board[startpoint.getRow(), startpoint.getColumn()]).setDoubleStep(currentGame.getTurnNumber());                   
                }
                if (board[endpoint.getRow(), endpoint.getColumn()] == null)
                {
                    if (endpoint.getColumn() == startpoint.getColumn() + 1)                   
                        board[startpoint.getRow(), endpoint.getColumn()] = null;
                    
                    if (endpoint.getColumn() == startpoint.getColumn() - 1)                   
                        board[startpoint.getRow(), endpoint.getColumn()] = null;
                }
                board[endpoint.getRow(), endpoint.getColumn()] = board[startpoint.getRow(), startpoint.getColumn()];
                board[startpoint.getRow(), startpoint.getColumn()] = null;
                if (endpoint.getRow() == 7 || endpoint.getRow() == 0)              
                    board[endpoint.getRow(), endpoint.getColumn()] = currentGame.coronation(board[endpoint.getRow(), endpoint.getColumn()]);
                
                if (currentGame.isCheck(currentGame,board[endpoint.getRow(), endpoint.getColumn()].getColor(), currentGame.getKingPlace(board, !board[endpoint.getRow(), endpoint.getColumn()].getColor()), board))                
                    Console.WriteLine("isCheck!!!!!!!!!!!!!!!!!!!!! You have to get out of this situation ");    
            }
            public override string toString() { return base.toString() + "P"; }

        }
        class Rook : ChessPiece
        {
            public Rook(bool isblack) : base(isblack) { }
            public override bool isLegalMove(ChessGame currentGame, bool color, Point startpoint, Point endpoint, ChessPiece[,] board)
            {
                bool isLegal = false;
                if (startpoint.getColumn() == endpoint.getColumn())//צריח זז למעלה או למטה
                {
                    int countofnull = 0;
                    int countofiteration = 0;
                    if (startpoint.getRow() > endpoint.getRow())//למעלה
                    {
                        for (int i = startpoint.getRow() - 1; i > endpoint.getRow(); i--)
                        {
                            if (board[i, startpoint.getColumn()] == null)
                                countofnull++;
                            countofiteration++;
                        }
                        if (countofiteration == countofnull)
                        {
                            if (board[endpoint.getRow(), endpoint.getColumn()] == null || board[endpoint.getRow(), endpoint.getColumn()].getColor() != board[startpoint.getRow(), startpoint.getColumn()].getColor())
                                isLegal = true;

                        }
                    }
                    if (startpoint.getRow() < endpoint.getRow())//למטה
                    {
                        for (int i = startpoint.getRow() + 1; i < endpoint.getRow(); i++)
                        {
                            if (board[i, startpoint.getColumn()] == null)
                                countofnull++;
                            countofiteration++;
                        }
                        if (countofiteration == countofnull)
                        {
                            if (board[endpoint.getRow(), endpoint.getColumn()] == null || board[endpoint.getRow(), endpoint.getColumn()].getColor() != board[startpoint.getRow(), startpoint.getColumn()].getColor())
                                isLegal = true;

                        }
                    }
                }
                if (startpoint.getRow() == endpoint.getRow())//צריח זז ימינה או שמאלה
                {
                    int countofnull = 0;
                    int countofiteration = 0;
                    if (startpoint.getColumn() > endpoint.getColumn())//שמאלה
                    {
                        for (int i = startpoint.getColumn() - 1; i > endpoint.getColumn(); i--)
                        {
                            if (board[startpoint.getRow(), i] == null)
                                countofnull++;
                            countofiteration++;
                        }
                        if (countofiteration == countofnull)
                        {
                            if (board[endpoint.getRow(), endpoint.getColumn()] == null || board[endpoint.getRow(), endpoint.getColumn()].getColor() != board[startpoint.getRow(), startpoint.getColumn()].getColor())
                                isLegal = true;
                        }
                    }
                    if (startpoint.getColumn() < endpoint.getColumn())//ימינה
                    {
                        for (int i = startpoint.getColumn() + 1; i < endpoint.getColumn(); i++)
                        {
                            if (board[startpoint.getRow(), i] == null)
                                countofnull++;
                            countofiteration++;
                        }
                        if (countofiteration == countofnull)
                        {
                            if (board[endpoint.getRow(), endpoint.getColumn()] == null || board[endpoint.getRow(), endpoint.getColumn()].getColor() != board[startpoint.getRow(), startpoint.getColumn()].getColor())
                                isLegal = true;
                        }
                    }
                }
                if (isLegal)
                {
                    ChessPiece tmppiece = currentGame.saveMoveForCheck(board, startpoint, endpoint);
                    if (currentGame.isCheck(currentGame, !color, currentGame.getKingPlace(board, color), board))                    
                        isLegal = false;
                    
                   currentGame. undoMove(board, startpoint, endpoint, tmppiece);
                }
                return isLegal;
            }
            public override void Move(ChessGame currentGame, bool isBlackTur ,Point startpoint, Point endpoint, ChessPiece[,] board)
            {
                board[startpoint.getRow(), startpoint.getColumn()].setNeverMoved(false);
                board[endpoint.getRow(), endpoint.getColumn()] = board[startpoint.getRow(), startpoint.getColumn()];
                board[startpoint.getRow(), startpoint.getColumn()] = null;
                if (currentGame.isCheck(currentGame,board[endpoint.getRow(), endpoint.getColumn()].getColor(), currentGame.getKingPlace(board, !board[endpoint.getRow(), endpoint.getColumn()].getColor()), board))                
                    Console.WriteLine("isCheck!!!!!!!!!!!!!!!!!!!!! You have to get out of this situation ");            
            }
            public override string toString() { return base.toString() + "R"; }
        }
        class Knight : ChessPiece
        {
            public Knight(bool isblack) : base(isblack) { }
            public override bool isLegalMove(ChessGame currentGame, bool color, Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                bool isLegal = false;
                if (((endPoint.getRow() == (startPoint.getRow() - 1)) || (endPoint.getRow() == (startPoint.getRow() + 1))) && ((endPoint.getColumn() == (startPoint.getColumn() - 2)) || (endPoint.getColumn() == (startPoint.getColumn() + 2))))
                {
                    if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                        isLegal = true;
                }
                if (((endPoint.getRow() == (startPoint.getRow() - 2)) || (endPoint.getRow() == (startPoint.getRow() + 2))) && ((endPoint.getColumn() == (startPoint.getColumn() - 1)) || (endPoint.getColumn() == (startPoint.getColumn() + 1))))
                {
                    if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                        isLegal = true;
                }
                if (isLegal == true)
                {
                    ChessPiece piece =currentGame. saveMoveForCheck(board, startPoint, endPoint);
                    if (currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board))                    
                        isLegal = false;                    
                   currentGame.undoMove(board, startPoint, endPoint, piece);
                }
                return isLegal;
            }
            public override void Move(ChessGame currentGame, bool isBlackTurn, Point startpoint, Point endpoint, ChessPiece[,] board)
            {
                board[startpoint.getRow(), startpoint.getColumn()].setNeverMoved(false);
                board[endpoint.getRow(), endpoint.getColumn()] = board[startpoint.getRow(), startpoint.getColumn()];
                board[startpoint.getRow(), startpoint.getColumn()] = null;
                if (currentGame.isCheck(currentGame,board[endpoint.getRow(), endpoint.getColumn()].getColor(), currentGame.getKingPlace(board,!board[endpoint.getRow(), endpoint.getColumn()].getColor()), board))               
                    Console.WriteLine("isCheck!!!!!!!!!!!!!!!!!!!!! You have to get out of this situation ");
                
            }
            public override string toString() { return base.toString() + "N"; }
        }
        class Bishop : ChessPiece
        {
            public Bishop(bool isblack) : base(isblack) { }
            public override bool  isLegalMove(ChessGame currentGame, bool color, Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                bool isLegal = false;
                if (startPoint.getColumn() > endPoint.getColumn())
                {
                    if (startPoint.getRow() > endPoint.getRow())//אלכסון שמאל למעלה
                    {
                        int countofnull = 0;
                        int countofiteration = 0;
                        int i = startPoint.getRow() - 1, j = startPoint.getColumn() - 1;
                        for (; i > endPoint.getRow() && j > endPoint.getColumn(); i--, j--)
                        {
                            if (board[i, j] == null)
                            {
                                countofnull++;
                            }
                            else
                            {
                                countofnull = 0;
                                countofiteration = 0;
                                break;
                            }
                            countofiteration++;
                        }
                        if ((countofiteration == countofnull && i == endPoint.getRow() && j == endPoint.getColumn()))
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                                isLegal = true;
                        }
                    }
                    if (startPoint.getRow() < endPoint.getRow())//אלכסון שמאל למטה
                    {
                        int countofnull = 0;
                        int countofiteration = 0;
                        int i = startPoint.getRow() + 1, j = startPoint.getColumn() - 1;
                        for (; i < endPoint.getRow() && j > endPoint.getColumn(); i++, j--)
                        {
                            if (board[i, j] == null)
                            {
                                countofnull++;
                            }
                            else
                            {
                                countofnull = 0;
                                countofiteration = 0;
                                break;
                            }

                            countofiteration++;
                        }
                        if ((countofiteration == countofnull && i == endPoint.getRow() && j == endPoint.getColumn()))
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                                isLegal = true;

                        }
                    }
                }
                if (startPoint.getColumn() < endPoint.getColumn())
                {
                    if (startPoint.getRow() > endPoint.getRow())//אלכסון ימין למעלה
                    {
                        int countofnull = 0;
                        int countofiteration = 0;
                        int i = startPoint.getRow() - 1, j = startPoint.getColumn() + 1;
                        for (; i > endPoint.getRow() && j < endPoint.getColumn(); i--, j++)
                        {
                            if (board[i, j] == null)
                            {
                                countofnull++;

                            }
                            else
                            {
                                countofnull = 0;
                                countofiteration = 0;
                                break;
                            }

                            countofiteration++;
                        }
                        if ((countofiteration == countofnull && i == endPoint.getRow() && j == endPoint.getColumn()))
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                                isLegal = true;

                        }
                    }
                    if (startPoint.getRow() < endPoint.getRow())//אלכסון ימין למטה
                    {
                        int countofnull = 0;
                        int countofiteration = 0;
                        int i = startPoint.getRow() + 1, j = startPoint.getColumn() + 1;
                        for (; i < endPoint.getRow() && j < endPoint.getColumn(); i++, j++)
                        {
                            if (board[i, j] == null)
                            {
                                countofnull++;

                            }
                            else
                            {
                                countofnull = 0;
                                countofiteration = 0;
                                break;
                            }
                            countofiteration++;
                        }
                        if ((countofiteration == countofnull && i == endPoint.getRow() && j == endPoint.getColumn()))
                        {
                            if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                                isLegal = true;

                        }
                    }
                }
                if (isLegal == true)
                {
                    ChessPiece piece = currentGame.saveMoveForCheck(board, startPoint, endPoint);
                    if (currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board))
                    {
                        isLegal = false;
                    }
                    currentGame.undoMove(board, startPoint, endPoint, piece);
                }
                return isLegal;
            }
            public override void Move(ChessGame currentGame, bool isBlackTurn ,Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                board[startPoint.getRow(), startPoint.getColumn()].setNeverMoved(false);
                board[endPoint.getRow(), endPoint.getColumn()] = board[startPoint.getRow(), startPoint.getColumn()];
                board[startPoint.getRow(), startPoint.getColumn()] = null;
                if (currentGame.isCheck(currentGame,board[endPoint.getRow(), endPoint.getColumn()].getColor(), currentGame.getKingPlace(board,!board[endPoint.getRow(), endPoint.getColumn()].getColor()), board))
                {
                    Console.WriteLine("isCheck!!!!!!!!!!!!!!!!!!!!! You have to get out of this situation ");
                }
            }
            public override string toString() { return base.toString() + "B"; }
        }
        class Queen : ChessPiece
        {
            public Queen(bool isblack) : base(isblack) { }
            public override bool isLegalMove(ChessGame currentGame, bool color, Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                bool isliegal = false;
                Rook tmprook = new Rook(board[startPoint.getRow(), startPoint.getColumn()].getColor());
                Bishop tmpbishop = new Bishop(board[startPoint.getRow(), startPoint.getColumn()].getColor());

                if (tmprook.isLegalMove( currentGame, tmprook.getColor(), startPoint, endPoint, board) || tmpbishop.isLegalMove( currentGame, tmpbishop.getColor(), startPoint, endPoint, board))
                    isliegal = true;

                return isliegal;

            }
            public override void Move(ChessGame currentGame, bool isBlackTurn, Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                board[startPoint.getRow(), startPoint.getColumn()].setNeverMoved(false);
                board[endPoint.getRow(), endPoint.getColumn()] = new Queen(board[startPoint.getRow(), startPoint.getColumn()].getColor());
                board[startPoint.getRow(), startPoint.getColumn()] = null;
                if (currentGame.isCheck(currentGame,board[endPoint.getRow(), endPoint.getColumn()].getColor(), currentGame.getKingPlace(board,!board[endPoint.getRow(), endPoint.getColumn()].getColor()), board))                
                    Console.WriteLine("isCheck!!!!!!!!!!!!!!!!!!!!! You have to get out of this situation ");                
            }
            public override string toString() { return base.toString() + "Q"; }
        }
        class King : ChessPiece
        {
            public King(bool isblack) : base(isblack)
            {

            }
            public override bool isLegalMove(ChessGame currentGame, bool color, Point startPoint, Point endPoint, ChessPiece[,] board)
            {
                bool isLegal = false;
                bool isCastling = false;
                if (startPoint.getRow() == endPoint.getRow() - 1)
                {
                    if (startPoint.getColumn() == endPoint.getColumn() + 1 || startPoint.getColumn() == endPoint.getColumn() - 1 || startPoint.getColumn() == endPoint.getColumn())
                    {
                        if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                            isLegal = true;
                    }
                }
                if (startPoint.getRow() == endPoint.getRow() + 1)
                {
                    if (startPoint.getColumn() == endPoint.getColumn() + 1 || startPoint.getColumn() == endPoint.getColumn() - 1 || startPoint.getColumn() == endPoint.getColumn())
                    {
                        if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                            isLegal = true;
                    }
                }
                if (startPoint.getRow() == endPoint.getRow())
                {
                    if (startPoint.getColumn() == endPoint.getColumn() + 1 || startPoint.getColumn() == endPoint.getColumn() - 1)
                    {
                        if (board[endPoint.getRow(), endPoint.getColumn()] == null || board[endPoint.getRow(), endPoint.getColumn()].getColor() != board[startPoint.getRow(), startPoint.getColumn()].getColor())
                            isLegal = true;
                    }
                    if (!(currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board)))
                    {
                        if (startPoint.getColumn() == endPoint.getColumn() - 2 && board[startPoint.getRow(), startPoint.getColumn()].getNeverMoved() == true)
                        {
                            if (board[startPoint.getRow(), startPoint.getColumn() + 3] != null && board[startPoint.getRow(), startPoint.getColumn() + 3] is Rook && board[startPoint.getRow(), startPoint.getColumn() + 3].getNeverMoved())
                            {
                                if (board[startPoint.getRow(), startPoint.getColumn() + 1] == null && board[startPoint.getRow(), startPoint.getColumn() + 2] == null)
                                {
                                    Point tmppoint = new Point(startPoint.getRow(), startPoint.getColumn() + 1);
                                    ChessPiece tmppiece = currentGame.saveMoveForCheck(board, startPoint, tmppoint);
                                    if (!(currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board)))
                                    {
                                        Point tmppoint2 = new Point(startPoint.getRow(), startPoint.getColumn() + 1);
                                        Point starttmppoint2 = new Point(startPoint.getRow(), 7);
                                        ChessPiece tmppiece2 = currentGame.saveMoveForCheck(board, starttmppoint2, tmppoint2);
                                        if (!(currentGame.isCheck(currentGame, !color, currentGame.getKingPlace(board, color), board)))
                                        {
                                            isCastling = true;
                                            isLegal = true;
                                        }
                                        currentGame.undoMove(board, starttmppoint2, tmppoint2, tmppiece2);
                                    }
                                    currentGame.undoMove(board, startPoint, tmppoint, tmppiece);
                                }
                            }
                        }                        
                        if (startPoint.getColumn() == endPoint.getColumn() + 2 && board[startPoint.getRow(), startPoint.getColumn()].getNeverMoved() == true)
                        {
                            if (board[startPoint.getRow(), startPoint.getColumn() + 3] != null && board[startPoint.getRow(), startPoint.getColumn() + 3] is Rook && board[startPoint.getRow(), startPoint.getColumn() + 3].getNeverMoved())
                            {
                                if (board[startPoint.getRow(), startPoint.getColumn() - 4] != null && board[startPoint.getRow(), startPoint.getColumn() - 4] is Rook && board[startPoint.getRow(), startPoint.getColumn() - 4].getNeverMoved())
                                {
                                    if (board[startPoint.getRow(), startPoint.getColumn() - 1] == null && board[startPoint.getRow(), startPoint.getColumn() - 2] == null && board[startPoint.getRow(), startPoint.getColumn() - 3] == null)
                                    {

                                        Point tmppoint = new Point(startPoint.getRow(), startPoint.getColumn() - 1);
                                        ChessPiece tmppiece = currentGame.saveMoveForCheck(board, startPoint, tmppoint);
                                        if (!(currentGame.isCheck(currentGame, !color, currentGame.getKingPlace(board, color), board)))
                                        {
                                            Point tmppoint2 = new Point(startPoint.getRow(), startPoint.getColumn() - 1);
                                            Point starttmppoint2 = new Point(startPoint.getRow(), 0);
                                            ChessPiece tmppiece2 = currentGame.saveMoveForCheck(board, starttmppoint2, tmppoint2);
                                            if (!(currentGame.isCheck(currentGame, !color, currentGame.getKingPlace(board, color), board)))
                                            {
                                                isCastling = true;
                                                isLegal = true;
                                            }
                                            currentGame.undoMove(board, starttmppoint2, tmppoint2, tmppiece2);
                                        }
                                        currentGame.undoMove(board, startPoint, tmppoint, tmppiece);
                                    }
                                }                             
                            }
                        }
                    }
                }
                if (isLegal == true && isCastling == false)
                {
                    ChessPiece tmppiece = currentGame.saveMoveForCheck(board, startPoint, endPoint);
                    if (currentGame.isCheck(currentGame,!color, currentGame.getKingPlace(board, color), board))
                    {
                        isLegal = false;
                    }
                    currentGame.undoMove(board, startPoint, endPoint, tmppiece);
                }
                return isLegal;
            }
            public override void Move( ChessGame currentGame, bool isBlackTurn, Point startpoint, Point endpoint, ChessPiece[,] board)
            {
                board[startpoint.getRow(), startpoint.getColumn()].setNeverMoved(false);
                if (startpoint.getColumn() == endpoint.getColumn() + 2)
                {
                    board[startpoint.getRow(), 0].setNeverMoved(false);
                    board[startpoint.getRow(), startpoint.getColumn() - 1] = board[startpoint.getRow(), 0];
                    board[startpoint.getRow(), 0] = null;
                }
                else if (startpoint.getColumn() == endpoint.getColumn() - 2)
                {
                    board[startpoint.getRow(), 7].setNeverMoved(false);
                    board[startpoint.getRow(), startpoint.getColumn() + 1] = board[startpoint.getRow(), 7];
                    board[startpoint.getRow(), 7] = null;
                }
                board[endpoint.getRow(), endpoint.getColumn()] = board[startpoint.getRow(), startpoint.getColumn()];
                board[startpoint.getRow(), startpoint.getColumn()] = null;
            }
            public override string toString() { return base.toString() + "K"; }

        }
        class ChessPiece
        {
            private bool isBlack;
            bool neverMoved;
            public ChessPiece(bool color)
            {
                this.isBlack = color;
                this.neverMoved = true;

            }
            public bool getColor() { return this.isBlack; }
            public void setColor() { this.isBlack = !isBlack; }
            public virtual string toString()
            {
                if (this.isBlack)
                    return "B";
                else
                    return "W";
            }
            public virtual bool isLegalMove(ChessGame currentGame, bool isBlackTurn, Point startPoint, Point endPoint, ChessPiece[,] board) { return false; }
            public virtual void Move(ChessGame currentGame,bool isBlackTurn, Point startPoint, Point endPoint, ChessPiece[,] board) // sets the starting position to null
            {
                board[startPoint.getRow(), startPoint.getColumn()] = null;
                return;
            }
            public virtual void setNeverMoved(bool neverMoved)
            {
                this.neverMoved = neverMoved;
            }
            public virtual bool getNeverMoved()
            {
                return this.neverMoved;
            }
           
        }
    }
}   

   