public class Player
{
    public class PlayerData
    {
        public int debtA = 0;
        public int debtB = 0;
        public int debtC = 0;

        public int currentBudgetA = 0;
        public int currentBudgetB = 0;
        public int currentBudgetC = 0;

        public bool canCatchA = false;
        public bool canCatchB = false;
        public bool canCatchC = false;

        public PlayerData()
        {
            canCatchA = true;
            currentBudgetA = 10;
        }
    }

    public PlayerData pData = new PlayerData();
    public bool canBuyShipB;
    public bool canBuyShipC;
    public bool wantBuyShipB;
    public bool wantBuyShipC;

    private int shipPriceB = 5;
    private int shipPriceC = 10;
    private int priceMultiplierA = 4;
    private int priceMultiplierB = 2;
    private int priceMultiplierC = 1;

    public void RoundStart(RoundData rData)
    {
        pData.currentBudgetA = CheckDebt(rData.bankA, ref pData.debtA);
        pData.currentBudgetB = CheckDebt(rData.bankB, ref pData.debtB);
        pData.currentBudgetC = CheckDebt(rData.bankC, ref pData.debtC);

        ApplyStatics(ref pData.currentBudgetA, ref rData.maintananceA, ref pData.debtA);

        ApplyStatics(ref pData.currentBudgetB, ref rData.maintananceB, ref pData.debtB);

        ApplyStatics(ref pData.currentBudgetC, ref rData.maintananceC, ref pData.debtC);
    }

    internal void RoundStart(FishData fishData1, FishData fishData2, FishData fishData3)
    {
    }

    private void ApplyStatics(ref int currentBudget, ref int newStatic, ref int debt)
    {
        if (currentBudget - newStatic > 0)
        {
            currentBudget -= newStatic;
        }
        else
        {
            debt += ( newStatic - currentBudget );
            currentBudget = 0;
        }
    }

    private int CheckDebt(int bank, ref int debt)
    {
        if (bank - debt > 0)
        {
            bank -= debt;
            debt = 0;
        }
        else
        {
            debt -= bank;
            bank = 0;
        }

        return bank;
    }

    private void Update()
    {
        // Check if the player has enough mone to buy new ships
        if (pData.canCatchB && ( ( pData.currentBudgetA / priceMultiplierA )
                                 + ( pData.currentBudgetB / priceMultiplierB ) + ( pData.currentBudgetC / priceMultiplierC ) ) > shipPriceB)
        {
            canBuyShipB = true;
        }

        if (pData.canCatchC && ( ( pData.currentBudgetA / priceMultiplierA )
                                 + ( pData.currentBudgetB / priceMultiplierB ) + ( pData.currentBudgetC / priceMultiplierC ) ) > shipPriceC)
        {
            canBuyShipC = true;
        }

        if (wantBuyShipB)
        {
            if (pData.currentBudgetA >= shipPriceB * priceMultiplierA)
            {
                pData.currentBudgetA -= shipPriceB * priceMultiplierA;
                pData.canCatchB = true;
            }
            else if (pData.currentBudgetB >= shipPriceB * priceMultiplierB)
            {
                pData.currentBudgetB -= shipPriceB * priceMultiplierB;
                pData.canCatchB = true;
            }
            else if (pData.currentBudgetC >= shipPriceB * priceMultiplierC)
            {
                pData.currentBudgetC -= shipPriceB * priceMultiplierC;
                pData.canCatchB = true;
            }
            else
            {
                wantBuyShipB = false;
            }
        }

        if (wantBuyShipC)
        {
            if (pData.currentBudgetA >= shipPriceC * priceMultiplierA)
            {
                pData.currentBudgetA -= shipPriceC * priceMultiplierA;
                pData.canCatchC = true;
            }
            else if (pData.currentBudgetB >= shipPriceC * priceMultiplierB)
            {
                pData.currentBudgetB -= shipPriceC * priceMultiplierB;
                pData.canCatchC = true;
            }
            else if (pData.currentBudgetC >= shipPriceC * priceMultiplierC)
            {
                pData.currentBudgetC -= shipPriceC * priceMultiplierC;
                pData.canCatchC = true;
            }
            else
            {
                wantBuyShipC = false;
            }
        }
    }
}