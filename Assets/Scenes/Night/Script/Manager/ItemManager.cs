using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

// 구현해야 하는 아이템 15개
public enum ItemTypes
{
    //공격
    CentryBall = 0, ChargingReaper, DisasterDrone, MultiSlash, RailPiercer,
    //방어
    FirstAde = 5, Barrior, HologramTrick, AntiPhenet, RegenerationArmor,
    //보조
    GravityBind = 10, MoveBack, Booster, BioSnach, InterceptDrone
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }


    //아이템 사용을 여기서 할거임
    [SerializeField]
    GameObject characterParent;
    [SerializeField]
    Character character;
    [SerializeField]
    GameObject hitBox;


    [SerializeField]
    GameObject[] itemPrefabArr;

    //item 변수

    [SerializeField]
    Sprite[] multiSlasherSprite;

    //아이템 쿨타임 전용
    [SerializeField]
    GameObject[] nowItemUIArr;
    public Sprite[] itemIconArr;
    [SerializeField]
    List<Image> coolTimeImageArr;

    public List<Consumable> activeItemList;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        GetPlayerItemInfomations();
        InitItemIcons();
        StartItem();
    }


    void GetPlayerItemInfomations()
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.itemSlot; i++)
        {
            Consumable item = player.item[i];
            if (item.itemIdx == 0) continue;

            activeItemList.Add(item);
        }
    }

    // TODO: activeItemList를 활용해서 코드 개선할 것
    void InitItemIcons()
    {
        const int MAX_SLOT_COUNT = 3;
        Player player = GameManager.Instance.player;

        int playerSlotNumber = 0;
        for (int i = 0; i < MAX_SLOT_COUNT; i++)
        {
            Consumable item = null;

            for (; playerSlotNumber < player.itemSlot; playerSlotNumber++)
            {
                if (player.item[playerSlotNumber].itemIdx != 0)
                {
                    item = player.item[playerSlotNumber];
                    playerSlotNumber++;
                    break;
                }
            }

            if (item == null)
                nowItemUIArr[i].SetActive(false);
            else
                nowItemUIArr[i].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.consumableIcons[item.itemIdx];
        }
    }

    void StartItem()
    {
        for (int i = 0; i < activeItemList.Count; i++)
        {
            Consumable item = activeItemList[i];
            ItemTypes type = GetItemType(item);
            switch (type)
            {
                case ItemTypes.CentryBall:
                    StartCoroutine(CentryBall(item));
                    break;
                case ItemTypes.ChargingReaper:
                    StartCoroutine(ChargingReaper(item, i));
                    break;
                case ItemTypes.DisasterDrone:
                    StartCoroutine(DisasterDrone(item));
                    break;
                case ItemTypes.MultiSlash:
                    StartCoroutine(MultiSlashCoroutine(item, i));
                    break;
                case ItemTypes.RailPiercer:
                    StartCoroutine(RailPiercer(item, i));
                    break;
                case ItemTypes.FirstAde:
                    StartCoroutine(FirstAdeCoroutine(item, i));
                    break;
                case ItemTypes.Barrior:
                    StartCoroutine(BarriorCoroutine(item, i));
                    break;
                case ItemTypes.HologramTrick:
                    StartCoroutine(HologramTrickCoroutine(item, i));
                    break;
                case ItemTypes.AntiPhenet:
                    StartCoroutine(AntiPhenet(item));
                    break;
                case ItemTypes.RegenerationArmor:
                    StartCoroutine(RegenerationArmor(item));
                    break;
                case ItemTypes.GravityBind:
                    StartCoroutine(GravityBind(item));
                    break;
                case ItemTypes.MoveBack:
                    StartCoroutine(MoveBack(item, i));
                    break;
                case ItemTypes.Booster:
                    StartCoroutine(BoosterCoroutine(item, i));
                    break;
                case ItemTypes.BioSnach:
                    StartCoroutine(BioSnach(item));
                    break;
                case ItemTypes.InterceptDrone:
                    StartCoroutine(InterceptDroneCoroutine(item, i));
                    break;
            }
        }
    }

    private ItemTypes GetItemType(Consumable item)
    {
        return (ItemTypes)((item.itemIdx - 1) % 15);
    }



    // 센트리볼: 공격범위 내의 적을 자동으로 감지하여 공격속도로 공격력 피해를 입히는 광선을 발사하는 구체 드론.
    IEnumerator CentryBall(Consumable item)
    {
        GameObject centryBall = Instantiate(itemPrefabArr[1]);

        CentryBall centryBallScript = centryBall.GetComponent<CentryBall>();
        centryBallScript.InitializeCentryBall(item);
        centryBallScript.ActiveCentryBall();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(centryBall);
    }

    // 차징 리퍼: 적을 처치할 때마다 차징 게이지가 5만큼 상승. 차징 게이지가 100이 되면, 차징 게이지를 전부 소모하고 범위 7 내의 모든 적에게 13만큼의 피해를 입히는 전자동 낫
    IEnumerator ChargingReaper(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject chargingReaper = Instantiate(itemPrefabArr[2]);

        ChargingReaper chargingReaperScript = chargingReaper.GetComponent<ChargingReaper>();
        chargingReaperScript.InitializeChargingReaper(item, coolTimeImage);
        chargingReaperScript.ActiveChargingReaper();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(chargingReaper);
    }

    // 재앙 드론: 범위(공격 범위 비례) 내의 모든 적에게 매초마다 적 최대체력의 %만큼 피해를 입히는 초소형 드론.
    IEnumerator DisasterDrone(Consumable item)
    {
        GameObject disasterDrone = Instantiate(itemPrefabArr[3]);

        DisasterDrone droneScript = disasterDrone.GetComponent<DisasterDrone>();
        droneScript.InitializeDisasterDrone(item);
        droneScript.StartDisasterDrone();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(disasterDrone);
    }



    // 멀티 슬래시: 매 6초마다의 자동 공격이 적을 2번 베고, 바라보는 방향으로 검기를 1개 발사. 검기는 #ar# (공격범위 100%)만큼 날아가고, 검기에 적중하는 모든 적에게 #at# (공격력 40%) 만큼 피해를 입힘.
    IEnumerator MultiSlashCoroutine(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject multiSlash = Instantiate(itemPrefabArr[4]);

        MultiSlash multiSlashScript = multiSlash.GetComponent<MultiSlash>();
        multiSlashScript.InitializeMultiSlash(item, coolTimeImage);
        multiSlashScript.StartMultiSlash();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(multiSlash);
    }

    // 레일 피어서: 구역 전체를 관통하는 광선을 #as# (공격속도 10%)의 공격속도로 발사하는 자동 중화기. 광선에 닿은 모든 적에게 #at# (공격력 50%) 만큼 피해를 입힘.
    IEnumerator RailPiercer(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject railPiercer = Instantiate(itemPrefabArr[5]);

        RailPiercer railPiercerScript = railPiercer.GetComponent<RailPiercer>();
        railPiercerScript.InitializeRailPiercer(item, coolTimeImage);
        railPiercerScript.StartRailPiercer();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(railPiercer);
    }









    // 퍼스트 에이드: 현재 체력이 최대 체력의 40% 이하가 되면 3초에 걸쳐 #at# (공격력 40%) 만큼 체력을 회복. 한번 발동된 뒤에는 30초의 재사용 대기시간이 있음
    IEnumerator FirstAdeCoroutine(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject firstAde = Instantiate(itemPrefabArr[6]);

        FirstAde firstAdeScript = firstAde.GetComponent<FirstAde>();
        firstAdeScript.InitializeFirstAde(item, coolTimeImage);
        firstAdeScript.StartFirstAde();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(firstAde);
    }

    // 배리어: #at# (공격력 50%)만큼 보호막을 생성하는 장치. 보호막이 파괴되면 재생성되기 까지 #as# (90 ÷ 공격속도 15%) 초가 소요됨.
    IEnumerator BarriorCoroutine(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject barrior = Instantiate(itemPrefabArr[7]);
        Barrior barriorScript = barrior.GetComponent<Barrior>();
        barriorScript.InitializeBarrior(item, coolTimeImage);
        barriorScript.StartBarrior();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(barrior);
    }

    // 홀로그램 트릭: #as# (120 ÷ 공격속도 30%) 초마다 #ar# (공격범위 20%) 초간 피해 면역 상태로 만들어주는 교란 장치. 피격 판정, 투사체 적중은 일어나나 피해를 받지 않음.
    IEnumerator HologramTrickCoroutine(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject hologram = Instantiate(itemPrefabArr[8]);
        HologramTrick hologramScript = hologram.GetComponent<HologramTrick>();
        hologramScript.InitializeHologramTrick(item, coolTimeImage);
        hologramScript.StartHologramTrick();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(hologram);
    }

    // 안티 페넷: 데미지 경감율이 #at# (공격력 30%) %만큼 상승하는 인공 피부 조직.
    IEnumerator AntiPhenet(Consumable item)
    {
        GameObject antiPhenet = Instantiate(itemPrefabArr[9]);
        AntiPhenet antiPhenetScript = antiPhenet.GetComponent<AntiPhenet>();
        antiPhenetScript.InitializeAntiPhenet(item);
        antiPhenetScript.StartAntiPhenet();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        antiPhenetScript.EndAntiPhenet();
        Destroy(antiPhenet);
    }

    // 재생성 갑옷: 최대 체력이 #at# (공격력 10%) 만큼 상승하고 매초 체력을 #ar# (공격범위 10%)만큼 회복시켜주는 생체공학 갑옷.
    IEnumerator RegenerationArmor(Consumable item)
    {
        GameObject regenerationArmor = Instantiate(itemPrefabArr[10]);
        RegenerationArmor armorScript = regenerationArmor.GetComponent<RegenerationArmor>();
        armorScript.InitializeRegenerationArmor(item);
        armorScript.StartRegenerationArmor();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        armorScript.EndRegenerationArmor();
        Destroy(regenerationArmor);
    }

    // 그래비티 바인드: 범위 #ar# (공격범위 100%) 내 모든 적의 이동속도를 #as# (공격속도 50)%만큼 감소시키는 중력장.
    IEnumerator GravityBind(Consumable item)
    {
        GameObject gravityBind = Instantiate(itemPrefabArr[11]);
        GravityBind gravityBindScript = gravityBind.GetComponent<GravityBind>();
        gravityBindScript.InitializeGravityBind(item);
        gravityBindScript.StartGravityBind();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(gravityBind);
    }

    // 무브 백: 매 #as# (20 ÷ 공격속도 10%) 초마다의 자동 공격이 적을 #ar# (공격범위 50%)만큼 밀어냄.
    IEnumerator MoveBack(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject moveBack = Instantiate(itemPrefabArr[12]);
        MoveBack moveBackScript = moveBack.GetComponent<MoveBack>();
        moveBackScript.InitializeMoveBack(item, coolTimeImage);
        moveBackScript.StartMoveBack();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(moveBack);
    }


    // 부스터: 매 #as# (30 ÷ 공격속도 20%) 초마다 #ar# (공격범위 10%) 초 동안 #at# (공격력 40%)만큼 이동속도 향상되는 신발
    IEnumerator BoosterCoroutine(Consumable item, int itemSlotNumber)
    {
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject booster = Instantiate(itemPrefabArr[13]);
        Booster boosterScript = booster.GetComponent<Booster>();
        boosterScript.InitializeBooster(item, coolTimeImage);
        boosterScript.StartBooster();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(booster);
    }

    // 바이오 스내치: 자동공격 성공시마다 체력이 1만큼 회복.
    IEnumerator BioSnach(Consumable item)
    {
        GameObject bioSnach = Instantiate(itemPrefabArr[14]);
        BioSnach bioSnachScript = bioSnach.GetComponent<BioSnach>();
        bioSnachScript.InitializeBioSnach(item);
        bioSnachScript.StartBioSnach();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        bioSnachScript.EndBioSnach();
        Destroy(bioSnach);
    }

    // 요격 드론: 매 #as# (40 ÷ 공격속도 10%) 초마다 범위 #ar# (공격범위 70%) 내 모든 적대적인 투사체를 삭제하는 비행 드론
    IEnumerator InterceptDroneCoroutine(Consumable item, int itemSlotNumber)
    {
        Logger.Log("요격 드론");
        Image coolTimeImage = coolTimeImageArr[itemSlotNumber];
        coolTimeImage.gameObject.SetActive(true);

        GameObject interceptDrone = Instantiate(itemPrefabArr[15]);
        InterceptDrone interceptScript = interceptDrone.GetComponent<InterceptDrone>();
        interceptScript.InitializeInterceptDrone(item, coolTimeImage);
        interceptScript.StartInterceptDrone();

        yield return new WaitUntil(() => NightManager.Instance.isStageEnd);
        Destroy(interceptDrone);
    }


    IEnumerator SetCooltimeCoroutine(int coolTimeImageIdx, float getCoolTime)
    {
        coolTimeImageArr[coolTimeImageIdx].gameObject.SetActive(true);
        float nowTime = 0;
        while (coolTimeImageArr[coolTimeImageIdx].fillAmount > 0)
        {
            nowTime += Time.deltaTime;
            coolTimeImageArr[coolTimeImageIdx].fillAmount = 1 - nowTime / getCoolTime;
            yield return null;
        }
    }
}
