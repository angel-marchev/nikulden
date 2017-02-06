using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class FishData
{
    public bool CanCatch { get; set; }
    public bool WantToCatch { get; set; }
    public int BaitAmount { get; set; }
    public int NeptuneAmount { get; set; }
    public int NicolasAmount { get; set; }
    public bool Premonition { get; set; }
    public int AmountLeft { get; set; }
    public int SupportAmount { get; set; }
    public int UpgradeAmount { get; set; }
    public int NewlyCatched { get; set; }
    public int StorageAmount { get; set; }
    public int TotalQuantity { get; set; }

    private int Debt
    {
        get { return NeptuneAmount + NicolasAmount + BaitAmount - SupportAmount; }
    }

    public void Evolve()
    {
        AmountLeft = TotalQuantity - NeptuneAmount - NicolasAmount - BaitAmount - SupportAmount - UpgradeAmount;

        Random random = new Random();
        var randomCoefficient = ((float) random.Next(50, 200))/100;
        NewlyCatched = Convert.ToInt32(Math.Round(BaitAmount * randomCoefficient * ( 1.00 + NeptuneAmount * 0.10 )));
        UnityEngine.Debug.Log(string.Format("Bait:{0} New: {1} Coeff:{2}", BaitAmount, NewlyCatched, randomCoefficient));
        TotalQuantity = AmountLeft + NewlyCatched;
        BaitAmount = 0;
        NeptuneAmount = 0;
        NicolasAmount = 0;
    }
}