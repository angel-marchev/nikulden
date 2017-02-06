using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TankMovement : NetworkBehaviour
{
    public int m_PlayerNumber = 1;                // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public int m_LocalID = 1;

    [SyncVar]
    public FishData m_AData; 
    [SyncVar]
    public FishData m_BData; 
    [SyncVar]
    public FishData m_CData;

    public bool? _serverUpdatePending = null;
    [SyncVar]
    private CanvasAccessor m_canvasAccessor;
    [SyncVar]
    private float m_time;

    private void Start()
    {
        m_AData = new FishData();
        m_BData = new FishData();
        m_CData = new FishData();
    }

    private void Update()
    {
        if (m_canvasAccessor == null)
        {
            m_canvasAccessor = new CanvasAccessor();
        }

        if (!isLocalPlayer || GameManager.s_Instance.m_submitted)
            return;

        if (_serverUpdatePending.HasValue)
        {
            if (_serverUpdatePending == true)
            {
                UpdateUIFromModel();
                _serverUpdatePending = null;
            }
            else
            {
                _serverUpdatePending = true;
            }
        }

        if (GameManager.s_Instance.m_submitting)
        {
            GameManager.s_Instance.m_submitting = false;
            CmdUploadData();
            GameManager.s_Instance.m_submitted = true;
        }

        m_time += Time.deltaTime;
        GameManager.s_Instance.m_TimerDisplay.text = (10.00 -  m_time).ToString("F2");

        //else if (m_canvasAccessor.A.ValuesDiffer(m_AData) || m_canvasAccessor.B.ValuesDiffer(m_BData) || m_canvasAccessor.C.ValuesDiffer(m_CData))
        //{
        //    //CmdUploadData();
        //}
    }

    private void UpdateUIFromModel()
    {
        UnityEngine.Debug.Log("Updating from server");
        m_canvasAccessor.A.SetData(m_AData);
        m_canvasAccessor.B.SetData(m_BData);
        m_canvasAccessor.C.SetData(m_CData);
    }

    [Command]
    public void CmdUploadData()
    {
        FillData(m_AData, m_canvasAccessor.A);
        FillData(m_BData, m_canvasAccessor.B);
        FillData(m_CData, m_canvasAccessor.C);
    }

    private void FillData(FishData data, CanvasFishAccessor canvasValues)
    {
        data.AmountLeft = int.Parse(canvasValues.LeftAmount);
        data.BaitAmount = int.Parse(canvasValues.Bait);
        data.NeptuneAmount = int.Parse(canvasValues.Neptune);
        data.NewlyCatched = int.Parse(canvasValues.NewlyCatched);
        data.NicolasAmount = int.Parse(canvasValues.Nicolas);
        data.Premonition = canvasValues.Premonition;
        data.StorageAmount = int.Parse(canvasValues.Storage);
        data.SupportAmount = int.Parse(canvasValues.Support);
        data.TotalQuantity = int.Parse(canvasValues.TotalQuantity);
        data.UpgradeAmount = int.Parse(canvasValues.UpgradeCost);
    }

    [ClientRpc]
    public void RpcSetResultsData(int fishNumber, bool canCatch, int amountLeft, int baitAmount, int neptuneAmount, int nicolasAmount, bool premonition, int supportAmount, int upgradeAmount, int newlyCatched, int storage, int total)
    {
        UnityEngine.Debug.Log("Setting defaults");
        var data = fishNumber == 0 ? m_AData : ( fishNumber == 1 ? m_BData : m_CData );
        data.CanCatch = canCatch;
        data.AmountLeft = amountLeft;
        data.BaitAmount = baitAmount;
        data.NeptuneAmount = neptuneAmount;
        data.NewlyCatched = newlyCatched;
        data.NicolasAmount = nicolasAmount;
        data.Premonition = premonition;
        data.StorageAmount = storage;
        data.SupportAmount = supportAmount;
        data.TotalQuantity = total;
        data.UpgradeAmount = upgradeAmount;
    }

    [ClientRpc]
    public void RpcReset()
    {
        _serverUpdatePending = false;
        m_time = 0;
    }

    // This function is called at the start of each round to make sure each tank is set up correctly.
    public void SetDefaults()
    {
    }

    [ClientRpc]
    public void RpcEvolveData()
    {
        UnityEngine.Debug.Log("Evolving");
        m_AData.Evolve();
        m_BData.Evolve();
        m_CData.Evolve();
    }
}

internal class CanvasAccessor
{
    public CanvasAccessor()
    {
        A = new CanvasFishAccessor(GameManager.s_Instance.m_AInputFields, GameManager.s_Instance.m_ATexts, GameManager.s_Instance.m_AToggle);
        B = new CanvasFishAccessor(GameManager.s_Instance.m_BInputFields, GameManager.s_Instance.m_BTexts, GameManager.s_Instance.m_BToggle);
        C = new CanvasFishAccessor(GameManager.s_Instance.m_CInputFields, GameManager.s_Instance.m_CTexts, GameManager.s_Instance.m_CToggle);
    }

    public CanvasFishAccessor A { get; private set; }
    public CanvasFishAccessor B { get; private set; }
    public CanvasFishAccessor C { get; private set; }

}

internal class CanvasFishAccessor
{
    private readonly Text m_StorageLabel;
    private readonly Text m_NewlyCatchedLabel;
    private readonly Text m_TotalQuantityLabel;
    private readonly Text m_SupportLabel;
    private readonly Text m_UpgradeCostLabel;
    private readonly InputField m_BaitInput;
    private readonly InputField m_NeptuneInput;
    private readonly InputField m_NicolasInput;
    private readonly Toggle m_premonition;
    private readonly Text m_LeftAmountLabel;

    public CanvasFishAccessor(InputField[] mAInputFields, Text[] mATexts, Toggle mAToggle)
    {
        m_LeftAmountLabel = mATexts[0];
        m_NewlyCatchedLabel = mATexts[1];
        m_StorageLabel = mATexts[2];
        m_SupportLabel = mATexts[3];
        m_UpgradeCostLabel = mATexts[4];
        m_TotalQuantityLabel = mATexts[5];

        m_BaitInput = mAInputFields[0];
        m_NeptuneInput = mAInputFields[1];
        m_NicolasInput = mAInputFields[2];

        m_premonition = mAToggle;
    }

    public string Storage { get { return m_StorageLabel.text; } set { m_StorageLabel.text = value.ToString(); } }
    public string NewlyCatched { get { return m_NewlyCatchedLabel.text; } set { m_NewlyCatchedLabel.text = value.ToString(); } }
    public string TotalQuantity { get { return m_TotalQuantityLabel.text; } set { m_TotalQuantityLabel.text = value.ToString(); } }
    public string Support { get { return m_SupportLabel.text; } set { m_SupportLabel.text = value.ToString(); } }
    public string UpgradeCost { get { return m_UpgradeCostLabel.text; } set { m_UpgradeCostLabel.text = value.ToString(); } }
    public string Bait { get { return m_BaitInput.text; } set { m_BaitInput.text = value.ToString(); } }
    public string Neptune { get { return m_NeptuneInput.text; } set { m_NeptuneInput.text = value.ToString(); } }
    public string Nicolas { get { return m_NicolasInput.text; } set { m_NicolasInput.text = value.ToString(); } }
    public bool Premonition { get { return m_premonition.isOn; } set { m_premonition.isOn = value; } }
    public string LeftAmount { get { return m_LeftAmountLabel.text; } set { m_LeftAmountLabel.text = value.ToString(); } }

    public void SetData(FishData data)
    {
        LeftAmount = data.AmountLeft.ToString();
        NewlyCatched = data.NewlyCatched.ToString();
        Storage = data.StorageAmount.ToString();
        Support = data.SupportAmount.ToString();
        UpgradeCost = data.UpgradeAmount.ToString();
        TotalQuantity = data.TotalQuantity.ToString();

        Bait = data.BaitAmount.ToString();
        Neptune = data.NeptuneAmount.ToString();
        Nicolas = data.NicolasAmount.ToString();

        Premonition = data.Premonition;
    }
    
    public bool ValuesDiffer(FishData data)
    {
        return LeftAmount == data.AmountLeft.ToString()&&
        NewlyCatched == data.NewlyCatched.ToString()&&
        Storage == data.StorageAmount.ToString()&&
        Support == data.SupportAmount.ToString()&&
        UpgradeCost == data.UpgradeAmount.ToString()&&
        TotalQuantity == data.TotalQuantity.ToString()&&

        Bait == data.BaitAmount.ToString()&&
        Neptune == data.NeptuneAmount.ToString()&&
        Nicolas == data.NicolasAmount.ToString()&&

        Premonition == data.Premonition;
    }
}