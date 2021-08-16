/*A game of Black Jack.
Version 1.0: Base game with just dealing cards to the player, the dealer and seeing who wins. Aces are only worth 1
                The game can loop for as many rounds as the player wants.
Version 2.0: Added a betting mechanic. The player starts now with 1000$ and the game ends automaticaly when running
                out of money. The programm can handle now the doble value of aces as 11 or 1. Code refactorized.
Version 3.0: Expanded the beginning menu to show the rules. Implemented the possibility of splitting if the player
                gets matching values (not just cards) at the beginning of the round. The code got certainly a bit
                messy and there could be a bug here and there I haven't accounted for. Code needs refactorizing.
Version 3.5: Overall code overhaul and refactoring. Now everything is much nicer and tidier.
                Also some previous bugs relating to splitting were fixed.
Created by OuterGazer*/

using System;
using System.Collections.Generic;

class Black_Jack_eng
{
    static bool playerGotAnAce;
    static bool playerGotAnAceSplit;
    static bool firstSplitHandOver;
    static bool dealerGotAnAce;

    static string playerChoseToSplit = "N";
    static string playerChoseToHit;
    static string pickAnother;
    static string playAnotherRound;

    static int playerMoneyAmount = 1000;
    static int playerBet;
    static int valueIndex = 0;
    static int suitIndex = 0;
    static int counter;
    static int handValuePlayer;
    static int handValuePlayerSplit;
    static int handValueDealer;

    static Random pickedValue = new Random();
    static Random pickedSuit = new Random();

    //First two arrays are created to hold the card names as strings.
    //One array holds the suit and the other the value.
    //The subindex 0 will remained unused in all created arrays in this program.
    static string[] cardSuit = new string[5]
        {
            null, //value for Index 0
            "Spades", "Diamonds", "Clubs", "Hearts"
        };
    static string[] cardValue = new string[14]
        {
            null, //value for Index 0
            "Ace", "Two", "Three", "Four", "Five",
            "Six", "Seven", "Eight", "Nine", "Ten",
            "Jack", "Queen", "King"
        };

    //Now we create a 2-Dimensional array that will hold the actual numeric values to perform the calculations
    //Whenever the hand value of the dealer or player is calculated, it will be done using this array
    static int[,] cardDeckValues = new int[5, 14];

    //Now we create another 2-Dimensional array that will check that the random
    //number generator doesn't repeat cards
    //Whenever a card is dealt, its position in this array will be assigned "false"
    static bool[,] cardDeckRepeatCheck = new bool[5, 14];


    //As the last of the preparations we create 2 lists that will hold the subindexes of cards dealt
    //throughout the round in real time
    //This has been necessary as the first round 2 cards per player are dealt and we need to remember them
    static List<int> forCardValues = new List<int>();
    static List<int> forCardSuits = new List<int>();

    /// <summary>
    /// Method that picks a card at random. It checks it is not repeated and adds it to a list to be later used
    /// </summary>
    static void PickRandomCard()
    {
        valueIndex = pickedValue.Next(1, 14);  //A card will be picked randomly
        suitIndex = pickedSuit.Next(1, 5);

        if (cardDeckRepeatCheck[suitIndex, valueIndex] == false)    //This statement avoids the dealing of already dealt cards
        {
            while (cardDeckRepeatCheck[suitIndex, valueIndex] == false)
            {
                valueIndex = pickedValue.Next(1, 14);
                suitIndex = pickedSuit.Next(1, 5);
            }
        }

        forCardValues.Add(valueIndex);   //The cards are added to a list. These lists will be used to access
        forCardSuits.Add(suitIndex);     //the 2-Dimensional arrays
    }

    /// <summary>
    /// Method that calculates the value of the hand.
    /// </summary>
    /// <param name="ace">Holds if the incoming hand has an ace of vaue 11</param
    /// <param name="handValue">holds the current value of the hand of the player or dealer</param
    /// <param name="loopCounter">Holds the current value of i when the method was called within a loop</param>
    static void CalculateHand(ref bool ace, ref int handValue, int loopCounter)
    {
        //These if statements calculate the value of the hand with the new card, given the old value
        //It takes in account that the aces can be worth both 1 or 11
        if ((cardDeckValues[forCardSuits[loopCounter], forCardValues[loopCounter]] == 1) &&
            (ace == false) && (handValue < 11))
        {
            handValue += (10 + cardDeckValues[forCardSuits[loopCounter], forCardValues[loopCounter]]);
            ace = true;
        }
        else
        {
            handValue += cardDeckValues[forCardSuits[loopCounter], forCardValues[loopCounter]];
        }
        if ((ace == true) && (handValue >= 22))
        {
            handValue -= 10;
            ace = false;
        }

        cardDeckRepeatCheck[suitIndex, valueIndex] = false; //Marking the card as false we avoid that is dealt again within the round
    }

    /// <summary>
    /// The main method of the program. It chooses a card at random (suit and value separately)
    /// It has a control subloop to avoid dealing repeated cards
    /// It calculates the new value of the hand over the old value
    /// </summary>
    /// <param name="ace">Holds if the incoming hand has an ace of vaue 11</param
    /// <param name="handValue">holds the current value of the hand of the player or dealer</param
    /// <param name="loopCounter">Holds the current value of i when the method was called within a loop</param>
    static void DealACard(ref bool ace, ref int handValue, int loopCounter)
    {
        PickRandomCard();

        CalculateHand(ref ace, ref handValue, loopCounter);
    }

    /// <summary>
    /// This method allows us to escape the card dealing loop if we get a Black Jack or go over 21
    /// </summary>
    /// <param name="handValue">The hand value from either the player or the dealer</param>
    /// <param name="handValueP">The hand value of the player</param>
    static bool BreakFromTheDealingLoop(int handValue, int handValueP = 50) //initial value set to 50 so it doesn't affect the player
    {
        if (handValue >= 21)
        {
            return true;
        }
        
        //only applies to dealer. it will break from the loop when the value is higher than the player
        if (handValue >= handValueP) 
        {
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Throughout the programm, at different times the player will be prompted to answer [Y/N] to a question
    /// This method deals with the prompt and assigns the proper value to a string that will be used
    /// in the programm
    /// </summary>
    /// <param name="prompt">The question that will be shown to the player</param>
    /// <param name="playerInput">The answer that will be passed to the programm back, either Y or N</param>
    static void YesNoPrompt(string prompt, out string playerInput)
    {
        do
        {
            Console.Write(prompt);
            playerInput = Console.ReadLine().ToUpper();
            if ((playerInput != "Y") && (playerInput != "N"))
            {
                Console.WriteLine("Please write exclusively \"Y\" or \"N\".");
            }
            else
                break;
            Console.WriteLine();
        } while (true);
    }

    /// <summary>
    /// This method "shuffles" the card deck. It turns all values true.
    /// Each time a card is dealt its value will be turned "false"
    /// This method will be called at the beginning of the round
    /// </summary>
    /// <param name="deckArray"> This parameter will always take the bool array cardDeckRepeatCheck</param>
    static void deckShuffle()
    {
        for (int i = 1; i < cardDeckRepeatCheck.GetLength(0); i++)
        {
            for (int j = 1; j < cardDeckRepeatCheck.GetLength(1); j++)
            {
                cardDeckRepeatCheck[i, j] = true;
            }
        }
    }

    /// <summary>
    /// Method that fills the arrays with the values of the cards
    /// </summary>
    static void FillDeck()
    {
        for (int i = 1; i < cardDeckValues.GetLength(0); i++)  //Loop to fill card values from 1 to 10
        {
            for (int j = 1; j < 11; j++)
            {
                cardDeckValues[i, j] = j;
            }
        }
        for (int i = 1; i < cardDeckValues.GetLength(0); i++)  //Loop to fill all Jacks, Queens and Kings wih value 10
        {
            for (int j = 11; j < 14; j++)
            {
                cardDeckValues[i, j] = 10;
            }
        }
    }

    /// <summary>
    /// Method that sets all relevant variables to their standard values at the beginning of the game
    /// </summary>
    static void ResetGame()
    {
        handValuePlayer = 0;
        handValuePlayerSplit = 50;  //This 50 helps to simplify some if-elses used during the dealer's turn.
        handValueDealer = 0;

        playerChoseToSplit = "N";

        FillDeck();
        deckShuffle();

        forCardValues.Clear();
        forCardSuits.Clear();

        playerGotAnAce = false;
        playerGotAnAceSplit = false;
        firstSplitHandOver = false;
        dealerGotAnAce = false;
    }

    /// <summary>
    /// Method to prompt the player to place a bet
    /// </summary>
    static void PlaceBet()
    {
        Console.WriteLine("Let's begin the round. You currenty have " + playerMoneyAmount + " dollars.");
        Console.Write("Press Enter to continue.");
        Console.ReadLine();
        do
        {
            Console.Write("Please place your bet: ");
            while (true)    //loop to avoid the program breaking because the user didn't enter a number
            {
                try
                {
                    playerBet = int.Parse(Console.ReadLine());
                    break;
                }
                catch
                {
                    Console.Write("Please enter a numeric value: ");
                }
            }
            if ((playerBet < 50) || (playerBet > playerMoneyAmount))
            {
                Console.WriteLine("You have entered an invalid money amount.");
            }
            else
            {
                playerMoneyAmount -= playerBet;
                Console.WriteLine("You place a bet of " + playerBet + " dollars. Good luck!");
                Console.Write("Press Enter to continue.");
                Console.ReadLine();
                break;
            }
        } while (true);
    }

    /// <summary>
    /// This method compares the hand of the player and the dealer, decides the outcome and updates
    /// the amount of money of the player
    /// </summary>
    /// <param name="playerHand">The value of the player's hand whether it's the first hand or the split hand</param>
    /// <param name="dealerHand">The value of the dealer's hand</param>
    static void CompareHands(int playerHand, int dealerHand)
    {
        if (dealerHand > 21) //specific case if the dealer looses and we go straight to the end of the round
        {
            Console.WriteLine("The Dealer went over the limit. You won!\nYou get your bet back plus the same amount");
            playerMoneyAmount += (2 * playerBet);
            return;
        }

        if (playerHand > 21)
        {
            Console.WriteLine("You went over the limit. You lost!\nYou loose your bet.");
        }
        else
        {
            if ((playerHand == 21) && (dealerHand < 21))
            {
                Console.WriteLine("You got Black Jack! You won!\nYou get your bet back plus the same amount");
                playerMoneyAmount += (2 * playerBet);
                return;
            }
            if ((playerHand < 21) && (dealerHand == 21))
            {
                Console.WriteLine("The Dealer got Black Jack! You lost!\nYou loose your bet.");
                return;
            }
            if ((playerHand == 21) && (dealerHand == 21))
            {
                Console.WriteLine("You both got Black Jack! It's a tie!\nYou get your bet back.");
                playerMoneyAmount += playerBet;
                return;
            }
            if (playerHand > dealerHand)
            {
                Console.WriteLine("The sum of your hand is " + playerHand + ", against the " +
                    "value of the Dealer's hand: " + dealerHand + "\nYou won! You get your bet plus the same amount!");
                playerMoneyAmount += (2 * playerBet);
            }
            if (playerHand < dealerHand)
            {
                Console.WriteLine("The sum of your hand is " + playerHand + ", against the " +
                    "value of the Dealer's hand: " + dealerHand + "\nYou lost! You loose your bet!");
            }
            if (playerHand == dealerHand)
            {
                Console.WriteLine("The sum of your hand is " + playerHand + ", against the " +
                    "value of the Dealer's hand: " + dealerHand + "\nIt's a tie! You get your bet back!");
                playerMoneyAmount += playerBet;
            }
        }
    }

    /// <summary>
    /// The beginning of the programm.
    /// </summary>
    static void Main()
    {        
        Console.WriteLine("Welcome to our Black Jack table!\nCurrently you are the only player.\n" +
            "Rules are as follow:\n\t- Minimum bet is 50 dollars, only full dollars are accepted." +
            "\n\t- You can only split at the beginning " +
            "of the round when dealt matching values.\n\t\tWhen splitting your bet will automatically double." +
            "\n\t- Doubling down hasn't been implemented as of yet.\n\t- Dealer stands at soft 18." +
            "\n\t- Aces are worth both 11 or 1 points depending if your hand goes above 21." +
            "\n\t- Winning is paid 2:1, you get your bet back plus the same amount.\n" +
            "That's all for now. Have fun!\n\nPress Enter to continue.");
        Console.ReadLine();
        Console.Clear();

        do
        {
            ResetGame();

            //This is the beginning of the round, first the player will place a bet
            PlaceBet();
            
            //Now we deal 2 cards to the player and dealer.
            //The second dealer card will remain hidden to the player.
            Console.WriteLine("\nNow 2 cards will be dealt to you and the Dealer." +
                "\nPress the Enter key to continue.");
            Console.ReadLine();

            for (int i = 0; i < 2; i++)  //loop to deal cards to the player
            {
                DealACard(ref playerGotAnAce, ref handValuePlayer, i);
            }

            for (int i = 2; i < 4; i++) //loop to deal cards to the Dealer
            {
                DealACard(ref dealerGotAnAce, ref handValueDealer, i);                
            }

            Console.WriteLine("The cards have been dealt as follows: \n" +
                "You have the " + cardValue[forCardValues[0]] + " of " + cardSuit[forCardSuits[0]] + " and the " +
                cardValue[forCardValues[1]] + " of " + cardSuit[forCardSuits[1]]);
            Console.WriteLine("The Dealer has the " + cardValue[forCardValues[2]] + " of " + cardSuit[forCardSuits[2]]
                + " plus another card facing down.");
            Console.WriteLine("\nThe value of your hand is " + handValuePlayer + ".");

            if ((dealerGotAnAce == true) && (forCardValues[2] == 1))
            {
                Console.WriteLine("The value of the Dealer's hand so far is "
                + (cardDeckValues[forCardSuits[2], forCardValues[2]]+10) + ".");
            }
            else
            {
                Console.WriteLine("The value of the Dealer's hand so far is "
                + cardDeckValues[forCardSuits[2], forCardValues[2]] + ".");
            }

            //cardDeckValues[forCardSuits[0], forCardValues[0]] = 1; //These two lines are here to test splitting
            //cardDeckValues[forCardSuits[1], forCardValues[1]] = 1;

            //We check if the player got matching cards and give the option of splitting
            if ((cardDeckValues[forCardSuits[0], forCardValues[0]]) ==
                (cardDeckValues[forCardSuits[1], forCardValues[1]]))
            {
                if((playerMoneyAmount-playerBet) < 0)
                {
                    Console.WriteLine("\nYou have matching values, but you can't split because you don't" +
                        " have enough money.");
                }
                else
                {
                    YesNoPrompt("\nYou have matching values! Do you wish to split your hand? [Y/N]", out playerChoseToSplit);

                    if (playerChoseToSplit == "Y")
                    {
                        Console.WriteLine("You place a new bet of " + playerBet + " dollars.");
                        playerMoneyAmount -= playerBet;
                        //playerBet += playerBet;

                        Console.Write("\nYou will now get one card dealt for each hand... " +
                            "Press the Enter key to continue");
                        Console.ReadLine();

                        for (int i = 4; i < 6; i++)  //loop to deal 2 new cards to the player
                        {
                            DealACard(ref playerGotAnAce, ref handValuePlayer, i);
                        }

                        //We need to cover the case if the split cards are Aces. We override the actual value
                        //of handValuePlayer and store the value of the second hand in its own variable
                        handValuePlayer = 0;
                        playerGotAnAce = false;
                        CalculateHand(ref playerGotAnAce, ref handValuePlayer, 0);
                        CalculateHand(ref playerGotAnAce, ref handValuePlayer, 4);

                        handValuePlayerSplit = 0;
                        playerGotAnAceSplit = false;
                        CalculateHand(ref playerGotAnAceSplit, ref handValuePlayerSplit, 1);
                        CalculateHand(ref playerGotAnAceSplit, ref handValuePlayerSplit, 5);

                        Console.WriteLine("\nThe cards have been dealt as follows: \n" +
                            "Your first hand consists of the " + cardValue[forCardValues[0]] + " of " +
                            cardSuit[forCardSuits[0]] + " and the " + cardValue[forCardValues[4]] + " of " +
                            cardSuit[forCardSuits[4]]);

                        Console.Write("The value of your first hand is " + handValuePlayer + ". Press Enter to continue.");
                        Console.ReadLine();

                        Console.WriteLine("\nYour split hand consists of the " + cardValue[forCardValues[1]] + " of " +
                            cardSuit[forCardSuits[1]] + " and the " + cardValue[forCardValues[5]] + " of " +
                            cardSuit[forCardSuits[5]]);

                        Console.Write("The value of your split hand is " + handValuePlayerSplit + ". " +
                            "Press Enter to continue.");
                        Console.ReadLine();
                    }
                } 
            }

            //We ask he player if they want to pick more cards for any of the hands
            if (playerChoseToSplit == "Y")
            {
                counter = 6;  //This variable will be used to store more values in the lists beginning at subindex 6
                YesNoPrompt("Do you wish to pick another card for your first hand? [Y/N]", out playerChoseToHit);
                if (playerChoseToHit == "N")
                {
                    firstSplitHandOver = true;
                    YesNoPrompt("Do you wish to pick another card for your split hand? [Y/N]", out playerChoseToHit);
                }
            }
            else
            {
                counter = 4;  //This variable will be used to store more values in the lists beginning at subindex 4
                YesNoPrompt("Do you wish to pick another card? [Y/N]", out playerChoseToHit);
            }

            //This codeblock triggers if the players wishes more cards
            if (playerChoseToHit == "Y")
            {
                do
                {
                    Console.Write("\nThe Dealer hands you a new card... Press Enter to continue");
                    Console.ReadLine();

                    if (firstSplitHandOver == false) //deals a card for the first hand
                    {
                        DealACard(ref playerGotAnAce, ref handValuePlayer, counter);
                                                
                        Console.WriteLine("You have picked the " + cardValue[forCardValues[counter]] + " of "
                            + cardSuit[forCardSuits[counter]] + ".\n");

                        Console.WriteLine("The value of your hand so far is " + handValuePlayer + ".\n");
                    }
                    if (firstSplitHandOver == true) //deals a card for the split hand once we are done with the first
                    {
                        DealACard(ref playerGotAnAceSplit, ref handValuePlayerSplit, counter);
                                                
                        Console.WriteLine("You have picked the " + cardValue[forCardValues[counter]] + " of "
                            + cardSuit[forCardSuits[counter]] + ".\n");

                        Console.WriteLine("The value of your split hand so far is " + handValuePlayerSplit + ".\n");
                    }

                    //We check the first hand to see if we got >= 21, if split, we automatically jump to the split hand
                    if ((BreakFromTheDealingLoop(handValuePlayer) == true) && (firstSplitHandOver == false))
                    {
                        if((BreakFromTheDealingLoop(handValuePlayerSplit) == true) && (handValuePlayerSplit != 50))
                        {
                            break;
                        }
                        if (playerChoseToSplit == "Y")
                        {
                            firstSplitHandOver = true;
                            YesNoPrompt("Do you wish to pick a card for your split hand? [Y/N]", out pickAnother);
                            if (pickAnother == "Y")
                            {
                                counter += 1;
                                continue;
                            }                            
                        }
                        else
                        {
                            break; //if we don't split, this is the line that triggers. We break from the loop
                                   //and jump direct to the dealer's turn
                        }
                    }

                    //We check the split hand to see if we got >= 21
                    if ((BreakFromTheDealingLoop(handValuePlayerSplit) == false) && (firstSplitHandOver == true))
                    {
                        YesNoPrompt("Do you wish to pick another card for your split hand? [Y/N]", out pickAnother);
                        if (pickAnother == "Y")
                        {
                            counter += 1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if ((BreakFromTheDealingLoop(handValuePlayerSplit) == true) && (handValuePlayerSplit != 50))
                    {
                        break;
                    }

                    YesNoPrompt("Do you wish to pick another card for this hand? [Y/N]", out pickAnother);
                    
                    if ((pickAnother == "N") && (playerChoseToSplit == "Y"))
                    {
                        firstSplitHandOver = true;
                        counter += 1;
                        
                        YesNoPrompt("Do you wish to pick another card for your split hand? [Y/N]", out pickAnother);
                        if (pickAnother == "Y") continue;
                        break;
                    }

                    counter += 1;

                    } while (pickAnother == "Y");
                }

            /*Now is the turn of the Dealer, we show first the hidden card, then decide if more cards
             should be drawn*/
                        
            if(handValuePlayer < 22 || handValuePlayerSplit < 22)
            {
                Console.Write("\nNow is the Dealer's turn. The Dealer flips the card facing down..." +
                    " Press enter to continue");
                Console.ReadLine();
                Console.WriteLine("It's the " + cardValue[forCardValues[3]] + " of " + cardSuit[forCardSuits[3]] + "!");
                Console.WriteLine("The value of the Dealer's hand is " + handValueDealer + ".");

                while (handValueDealer < 18) //Loop for the Dealer's turn, at soft 18 no more cards will be drawn.
                {
                    Console.WriteLine("\nThe Dealer picks another card... Press Enter to continue.");
                    Console.ReadLine();

                    DealACard(ref dealerGotAnAce, ref handValueDealer, counter);

                    Console.WriteLine("The Dealer picked the " + cardValue[forCardValues[counter]] + " of "
                        + cardSuit[forCardSuits[counter]] + ".");

                    Console.WriteLine("The value of the Dealer's hand so far is " + handValueDealer + ".");

                    if (BreakFromTheDealingLoop(handValueDealer, handValuePlayer) == true)
                    {
                        if(BreakFromTheDealingLoop(handValueDealer, handValuePlayerSplit) == true)
                        {
                            Console.WriteLine("The turn of the dealer has finished! Let's see the outcome.");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine(); //We stop the game momentarily so the outcome doesn't just suddenly pop up
                                                //this improves game flow to help the player not feel overwhelmed
                            break;
                        }
                        if(playerChoseToSplit == "N") //"N" is the standard value. This if triggers in normal plays
                        {
                            Console.WriteLine("The turn of the dealer has finished! Let's see the outcome.");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        counter += 1; //These 2 lines trigger if we have split but the dealer's hand is still smaller
                        continue;     //than the split hand (although >= than the normal hand)
                    }

                    counter += 1;
                }
            }

            Console.WriteLine();    //adds a blank line to improve readability.            

            //Here are the values of the Player's and Dealer's hand compared to determine the outcome            
            if (playerChoseToSplit == "Y")
            {
                Console.WriteLine();
                Console.WriteLine("For your first hand... Press Enter to continue: ");
                Console.ReadLine();
                CompareHands(handValuePlayer, handValueDealer);

                Console.WriteLine();
                Console.WriteLine("For your split hand... Press Enter to continue: ");
                Console.ReadLine();
                CompareHands(handValuePlayerSplit, handValueDealer);
            }
            else
            {
                CompareHands(handValuePlayer, handValueDealer);
            }      

            //We have reached the end of the round and we go back if the player wants another round.
            if (playerMoneyAmount <= 0)
            {
                Console.WriteLine("\nI'm afraid that you have no money left. You must leave the table now.");
                break;
            }

            YesNoPrompt("\nDo you wish to play another round? [Y/N]", out playAnotherRound);
            
            Console.Clear();

            if (playAnotherRound == "N")
            {
                Console.WriteLine("You leave the table with " + playerMoneyAmount + " dollars.\nSee you next time!");
            }

        } while (playAnotherRound == "Y");

        Console.WriteLine("\nGame over!\nPress return to exit to Windows.");
        Console.ReadLine();
    }   
}