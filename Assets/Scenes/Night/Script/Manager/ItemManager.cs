using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    int[] itemIdxArr;
    [SerializeField]
    int[] itemRankArr;
    [SerializeField]
    int[] tempItemIdxArr;

    [SerializeField]
    GameObject[] itemPrefabArr;

    //item 변수
    [SerializeField]
    float centryBallAngle;
    [SerializeField]
    float chargingReaperAngle;
    [SerializeField]
    float reaperCircleR = 3; //반지름
    float reaperDeg = 0; //각도
    [SerializeField]
    Sprite[] multiSlasherSprite;
    [SerializeField]
    float reaperSpeed = 600; //원운동 속도
    ChargingReaper chargingReaperScript;
    public bool isChargingReaperUse = false;

    //아이템 쿨타임 전용
    [SerializeField]
    GameObject[] nowItemUIArr;
    public Sprite[] itemIconArr;
    [SerializeField]
    List<Image> coolTimeImageArr;

    public List<Consumable> activeItemList;

    //사운드 변수
    bool isReaperSoundPlay = false;

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

            itemIdxArr[i] = item.itemIdx;
            itemRankArr[i] = item.rank;
            itemIdxArr[i] -= (itemRankArr[i] * 15);     //아이템 인덱스로 ItemName을 구분하기 때문에 강제로 만든 식입니다.
                                                        //아이템 인덱스 - 랭크*15 = itemName
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
                    break;
                }
            }

            if (item  == null)
                nowItemUIArr[i].SetActive(false);
            else
                nowItemUIArr[i].transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.consumableIcons[item.itemIdx];
        }
    }

    void StartItem()
    {
        foreach (Consumable item in activeItemList)
        {
            ItemTypes type = GetItemType(item);
            int getRank = item.rank;
            switch (type)
            {
                case ItemTypes.CentryBall:
                    StartCoroutine(CentryBallCoroutine(getRank));
                    break;
                case ItemTypes.ChargingReaper:
                    StartCoroutine(ChargingReaperCoroutine(getRank));
                    break;
                case ItemTypes.DisasterDrone:
                    DisasterDrone(getRank);
                    break;
                case ItemTypes.MultiSlash:
                    StartCoroutine(MultiSlashCoroutine(getRank));
                    break;
                case ItemTypes.RailPiercer:
                    RailPiercer(getRank);
                    break;
                case ItemTypes.FirstAde:
                    StartCoroutine(FirstAdeCoroutine(getRank));
                    break;
                case ItemTypes.Barrior:
                    StartCoroutine(BarriorCoroutine(getRank));
                    break;
                case ItemTypes.HologramTrick:
                    StartCoroutine(HologramTrickCoroutine(getRank));
                    break;
                case ItemTypes.AntiPhenet:
                    AntiPhenet(getRank);
                    break;
                case ItemTypes.RegenerationArmor:
                    RegenerationArmor(getRank);
                    break;
                case ItemTypes.GravityBind:
                    GravityBind(getRank);
                    break;
                case ItemTypes.MoveBack:
                    StartCoroutine(MoveBack(getRank));
                    break;
                case ItemTypes.Booster:
                    StartCoroutine(BoosterCoroutine(getRank));
                    break;
                case ItemTypes.BioSnach:
                    BioSnach(getRank);
                    break;
                case ItemTypes.InterceptDrone:
                    InterceptDroneCoroutine(getRank);
                    break;
            }
        }
    }

    private ItemTypes GetItemType(Consumable item)
    {
        return (ItemTypes)(item.itemIdx % 15);
    }


    











    IEnumerator CentryBallCoroutine(int getitemRank)
    {
        GameObject centryBallParent = Instantiate(itemPrefabArr[1]);
        GameObject centryBall = centryBallParent.transform.GetChild(0).gameObject;
        CentryBall centryBallScript = centryBallParent.GetComponent<CentryBall>();
        centryBallParent.transform.SetParent(character.transform);
        centryBallScript.SetItemRank(getitemRank);

        Vector3 centryBallPos = character.transform.position;
        centryBallPos.y += 7;

        centryBall.transform.localPosition = centryBallPos;
        while (!NightManager.Instance.isStageEnd)
        {
            if (Time.timeScale != 0)
            {
                centryBall.transform.RotateAround(character.transform.position, Vector3.back, centryBallAngle);
                if (!centryBallScript.StopCentryBall())
                {
                    centryBall.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                }
            }
            yield return null;
        }
    }

    IEnumerator ChargingReaperCoroutine(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.ChargingReaper);

        isChargingReaperUse = true;
        GameObject chargingReaperParent = Instantiate(itemPrefabArr[2]);
        chargingReaperParent.transform.SetParent(characterParent.transform);
        chargingReaperScript = chargingReaperParent.GetComponent<ChargingReaper>();
        chargingReaperScript.SetItemRank(getRank);

        Transform reaperImageTransform = chargingReaperParent.transform.GetChild(0);
        StartCoroutine(chargingReaperScript.SetCoolTime(coolTimeImageArr[iconIdx]));

        while (!NightManager.Instance.isStageEnd)
        {
            if (chargingReaperScript.IsChargingGaugeFull())
            {
                if (!isReaperSoundPlay)
                {
                    isReaperSoundPlay = true;
                    NightSFXManager.Instance.PlayAudioClip(AudioClipName.chargingReaper);
                }
                reaperDeg += Time.deltaTime * reaperSpeed;
                if (reaperDeg < 360)
                {
                    var rad = Mathf.Deg2Rad * (reaperDeg);
                    var x = reaperCircleR * Mathf.Sin(rad);
                    var y = reaperCircleR * Mathf.Cos(rad);
                    reaperImageTransform.transform.position = character.transform.position + new Vector3(x, y);
                    reaperImageTransform.transform.rotation = Quaternion.Euler(0, 0, reaperDeg * -1); //가운데를 바라보게 각도 조절
                }
                else
                {
                    reaperDeg = 0;
                    chargingReaperScript.ReaperUse();
                    chargingReaperScript.chargingGauge -= 100;
                }
                yield return null;
            }
            else
            {
                if (isReaperSoundPlay)
                    isReaperSoundPlay = false;
                coolTimeImageArr[iconIdx].fillAmount = 1 - (float)chargingReaperScript.chargingGauge / 100;
                yield return new WaitUntil(() => chargingReaperScript.IsChargingGaugeFull());
            }
        }
    }

    public void ChargingReaperGauge()
    {
        if (isChargingReaperUse)
            chargingReaperScript.ChargingGauge();
    }

    void DisasterDrone(int getRank)
    {
        GameObject disasterDroneParent = Instantiate(itemPrefabArr[3]);
        disasterDroneParent.transform.SetParent(character.transform);
        disasterDroneParent.transform.localPosition = character.transform.position;
        disasterDroneParent.GetComponent<DisasterDrone>().character = character;
        disasterDroneParent.GetComponent<DisasterDrone>().nightManager = NightManager.Instance;
        disasterDroneParent.GetComponent<DisasterDrone>().SetItemRank(getRank);
    }

    IEnumerator MultiSlashCoroutine(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.MultiSlash);

        //랭크에 따라 다른 작업 추가
        float slashTime = 6;
        float attackPowerRate = (float)DataManager.Instance.consumableList.item[3].attackPowerValue;
        float slashAttackPower = (float)character.player.attackPower * attackPowerRate;
        float attackRange = (float)character.player.attackRange;

        switch (getRank)
        {
            case 0:
                slashTime = 6;
                break;
            case 1:
                slashTime = 5;
                break;
            case 2:
                slashTime = 4;
                break;
            case 3:
                slashTime = 3;
                break;
        }

        //2회 연속공격 + 에너지파
        while (!NightManager.Instance.isStageEnd)
        {
            character.Attack.isDoubleAttack = true;

            while (character.Attack.isDoubleAttack)
            {
                yield return null;
            }
            Debug.Log("end");

            NightSFXManager.Instance.PlayAudioClip(AudioClipName.multiSlash);
            ShootSwordAura(slashAttackPower, attackRange, getRank);

            if (coolTimeImageArr[iconIdx].fillAmount == 0)
                coolTimeImageArr[iconIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(iconIdx, slashTime));

            yield return new WaitForSeconds(slashTime);
        }
    }

    public void SetMultiSlasherSprite(bool isMultiActive)
    {
        SpriteRenderer hitBoxSpriteRenderer = hitBox.GetComponent<SpriteRenderer>();

        if (isMultiActive)
            hitBoxSpriteRenderer.sprite = multiSlasherSprite[0];   //멀티 슬레쉬 스프라이트
        else
            hitBoxSpriteRenderer.sprite = multiSlasherSprite[1];   //멀티 슬레쉬 스프라이트
    }

    void ShootSwordAura(float getSlashAttackPower, float getSlashAttackRange, int getItemRank)
    {
        GameObject swordAura = Instantiate(itemPrefabArr[4]);
        swordAura.transform.SetParent(characterParent.transform);
        swordAura.transform.position = hitBox.transform.position;
        swordAura.GetComponent<SwordAura>().attackDamage = getSlashAttackPower;
        swordAura.GetComponent<SwordAura>().attackRange = getSlashAttackRange;
        swordAura.GetComponent<SwordAura>().itemRank = getItemRank;

        //Color swordAuraColor = Color.blue;
        //swordAuraColor.a = 0.5f;

        //swordAura.GetComponent<SpriteRenderer>().color = swordAuraColor;
        SetSwordAuraAngle(swordAura);

        if (character.Movement.LastMoveDirection != Vector2.zero)
            swordAura.GetComponent<Rigidbody2D>().linearVelocity = character.Movement.LastMoveDirection.normalized * 10;
        else
            swordAura.GetComponent<Rigidbody2D>().linearVelocity = Vector3.right * 10;
    }

    void SetSwordAuraAngle(GameObject getSwordAura)
    {
        if (character.Movement.LastMoveDirection != Vector2.zero)
        {
            float dot = Vector3.Dot(character.Movement.LastMoveDirection, new Vector3(1, 0, 0));
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (character.Movement.LastMoveDirection.y >= 0)
                getSwordAura.transform.rotation = Quaternion.Euler(0, 0, angle);
            else
                getSwordAura.transform.rotation = Quaternion.Euler(0, 0, 360 - angle);
        }
        else
        {
            getSwordAura.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void RailPiercer(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.RailPiercer);

        GameObject railPiercerParent = Instantiate(itemPrefabArr[5]);
        railPiercerParent.transform.SetParent(characterParent.transform);
        RailPiercer railPiercerScript = railPiercerParent.GetComponent<RailPiercer>();

        railPiercerScript.character = character;
        railPiercerScript.nightManager = NightManager.Instance;
        railPiercerScript.nightSFXManager = NightSFXManager.Instance;

        railPiercerScript.coolTimeImage = coolTimeImageArr[iconIdx];
        railPiercerScript.SetItemRank(getRank);

        railPiercerScript.SetRailPiercerPos();
        railPiercerScript.ShootRailPiercer();
    }

    IEnumerator FirstAdeCoroutine(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.FirstAde);

        GameObject firstAdeParent = Instantiate(itemPrefabArr[6]);
        firstAdeParent.transform.SetParent(character.transform);
        firstAdeParent.transform.localPosition = character.transform.position;
        firstAdeParent.SetActive(false);

        double nowHp = GameManager.Instance.player.curHp;
        double firstAdeHp = character.player.maxHp * 0.4f;
        float healHp = (float)character.player.attackPower;
        int coolTime = 30;

        switch (getRank)
        {
            case 0:
                healHp *= (float)DataManager.Instance.consumableList.item[5].attackPowerValue;
                coolTime = 30;
                break;
            case 1:
                healHp *= (float)DataManager.Instance.consumableList.item[20].attackPowerValue;
                coolTime = 25;
                break;
            case 2:
                healHp *= (float)DataManager.Instance.consumableList.item[35].attackPowerValue;
                coolTime = 20;
                break;
            case 3:
                healHp *= (float)DataManager.Instance.consumableList.item[50].attackPowerValue;
                coolTime = 15;
                break;
        }

        while (!NightManager.Instance.isStageEnd)
        {
            //왜인지는 모르겠는데 현재 체력이 0으로 받아오는 버그가 있음 근데 이유를 모르겠음;;
            while (nowHp == 0)
            {
                nowHp = GameManager.Instance.player.curHp;
                yield return null;
            }

            while (nowHp >= firstAdeHp)
            {
                nowHp = GameManager.Instance.player.curHp;
                yield return null;
            }
            Debug.Log("helldfaiodhsfoiasdbfodsabfioads");
            coolTimeImageArr[iconIdx].fillAmount = 1;
            character.HealHp(healHp, firstAdeParent);

            NightSFXManager.Instance.PlayAudioClip(AudioClipName.firstAde);
            StartCoroutine(SetCooltimeCoroutine(iconIdx, coolTime));
            yield return new WaitForSeconds(coolTime);
        }
    }

    IEnumerator BarriorCoroutine(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.Barrior);

        GameObject barriorParent = Instantiate(itemPrefabArr[7]);
        barriorParent.transform.SetParent(character.transform);
        barriorParent.transform.position = character.transform.position;
        Barrior barriorScript = barriorParent.GetComponent<Barrior>();

        float characterAttackSpeed = (float)character.player.attackSpeed;
        float characterAttackPower = (float)character.player.attackPower;
        float shieldPoint = characterAttackPower;

        double timeCount = 50 / characterAttackSpeed;

        switch (getRank)
        {
            case 0:
                shieldPoint *= (float)DataManager.Instance.consumableList.item[6].attackPowerValue;
                timeCount = 90 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[6].attackSpeedValue);
                break;
            case 1:
                shieldPoint *= (float)DataManager.Instance.consumableList.item[6].attackPowerValue;
                timeCount = 90 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[21].attackSpeedValue);
                break;
            case 2:
                shieldPoint *= (float)DataManager.Instance.consumableList.item[6].attackPowerValue;
                timeCount = 90 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[36].attackSpeedValue);
                break;
            case 3:
                shieldPoint *= (float)DataManager.Instance.consumableList.item[6].attackPowerValue;
                timeCount = 90 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[51].attackSpeedValue);
                break;
        }

        while (!NightManager.Instance.isStageEnd)
        {
            character.SetShieldPointData(shieldPoint);
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.barrior);
            barriorScript.CreateBarrior();

            yield return new WaitUntil(() => character.player.shieldPoint == 0);
            barriorScript.SetBarriorActive(false);
            Debug.Log(timeCount);
            if (coolTimeImageArr[iconIdx].fillAmount == 0)
                coolTimeImageArr[iconIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(iconIdx, (float)timeCount));
            yield return new WaitForSeconds((float)timeCount);
        }
    }

    IEnumerator HologramTrickCoroutine(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.HologramTrick);

        GameObject[] hologramParentArr = new GameObject[2];
        Vector3 hologramVector;
        character.isHologramAnimate = true;

        for (int i = 0; i < 2; i++)
        {
            hologramVector = character.transform.position;
            GameObject hologramParent = Instantiate(itemPrefabArr[8]);
            hologramParent.transform.SetParent(character.transform);
            hologramParent.transform.position = character.transform.position;
            hologramParentArr[i] = hologramParent;

            if (i == 0)
            {
                hologramVector.x -= 1;
            }
            else
            {
                hologramVector.x += 1;
            }

            hologramParentArr[i].transform.position = hologramVector;
            character.hologramAnimatorArr[i] = hologramParent.GetComponent<Animator>();
            character.hologramRendererArr[i] = hologramParent.GetComponent<SpriteRenderer>();
        }

        float characterAttackSpeed = (float)character.player.attackSpeed;
        float characterAttackRange = (float)character.player.attackRange;

        float duration = characterAttackRange;
        float timeCount = 1200 / characterAttackSpeed;

        switch (getRank)
        {
            case 0:
                timeCount = 120 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[7].attackSpeedValue);
                duration = characterAttackRange * (float)DataManager.Instance.consumableList.item[7].attackRangeValue;
                break;
            case 1:
                timeCount = 120 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[22].attackSpeedValue);
                duration = characterAttackRange * (float)DataManager.Instance.consumableList.item[22].attackRangeValue;
                break;
            case 2:
                timeCount = 120 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[37].attackSpeedValue);
                duration = characterAttackRange * (float)DataManager.Instance.consumableList.item[37].attackRangeValue;
                break;
            case 3:
                timeCount = 120 / (characterAttackSpeed * (float)DataManager.Instance.consumableList.item[52].attackSpeedValue);
                duration = characterAttackRange * (float)DataManager.Instance.consumableList.item[52].attackRangeValue;
                break;
        }

        timeCount *= -1;

        while (!NightManager.Instance.isStageEnd)
        {
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.hologramTrick);
            character.isHologramTrickOn = true;
            yield return new WaitForSeconds(duration);
            character.isHologramTrickOn = false;

            if (coolTimeImageArr[iconIdx].fillAmount == 0)
                coolTimeImageArr[iconIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(iconIdx, timeCount));
            yield return new WaitForSeconds(timeCount);
        }

        character.isHologramAnimate = false;
    }

    void AntiPhenet(int getRank)
    {
        character.isAntiPhenetOn = true;

        switch (getRank)
        {
            case 0:
                character.SetAntiPhenetData((float)DataManager.Instance.consumableList.item[8].attackPowerValue);
                break;
            case 1:
                character.SetAntiPhenetData((float)DataManager.Instance.consumableList.item[23].attackPowerValue);
                break;
            case 2:
                character.SetAntiPhenetData((float)DataManager.Instance.consumableList.item[38].attackPowerValue);
                break;
            case 3:
                character.SetAntiPhenetData((float)DataManager.Instance.consumableList.item[53].attackPowerValue);
                break;
        }
    }

    void RegenerationArmor(int getRank)
    {

        float addHp = (float)character.player.attackPower;
        float addHpRegen = (float)character.player.attackRange;

        switch (getRank)
        {
            case 0:
                addHp *= (float)DataManager.Instance.consumableList.item[9].attackPowerValue;
                addHpRegen *= (float)DataManager.Instance.consumableList.item[9].attackRangeValue;
                Debug.Log(addHpRegen);
                break;
            case 1:
                addHp *= (float)DataManager.Instance.consumableList.item[24].attackPowerValue;
                addHpRegen *= (float)DataManager.Instance.consumableList.item[24].attackRangeValue;
                break;
            case 2:
                addHp *= (float)DataManager.Instance.consumableList.item[39].attackPowerValue;
                addHpRegen *= (float)DataManager.Instance.consumableList.item[39].attackRangeValue;
                break;
            case 3:
                addHp *= (float)DataManager.Instance.consumableList.item[54].attackPowerValue;
                addHpRegen *= (float)DataManager.Instance.consumableList.item[54].attackRangeValue;
                break;
        }

        character.Health.HealToMaxHp();
        character.player.hpRegen = addHpRegen;
    }

    void GravityBind(int getRank)
    {
        GameObject gravityBindParent = Instantiate(itemPrefabArr[11]);
        gravityBindParent.transform.SetParent(characterParent.transform);
        gravityBindParent.transform.localPosition = character.transform.position;
        gravityBindParent.transform.GetChild(0).GetComponent<GravityBind>().character = character;
        gravityBindParent.transform.GetChild(0).GetComponent<GravityBind>().nightManager = NightManager.Instance;
        gravityBindParent.transform.GetChild(0).GetComponent<GravityBind>().SetItemRank(getRank);
    }

    IEnumerator MoveBack(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.MoveBack);

        float getAttackSpeed = (float)character.player.attackSpeed;
        float timeCount = 200 / getAttackSpeed;

        switch (getRank)
        {
            case 0:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[11].attackSpeedValue);
                break;
            case 1:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[26].attackSpeedValue);
                break;
            case 2:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[41].attackSpeedValue);
                break;
            case 3:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[56].attackSpeedValue);
                break;
        }

        timeCount *= -1;

        while (!NightManager.Instance.isStageEnd)
        {
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.moveBack);
            character.isMoveBackOn = true;
            GameObject moveBackImage = Instantiate(itemPrefabArr[12]);
            moveBackImage.GetComponent<MoveBackImage>().character = character.transform;

            if (coolTimeImageArr[iconIdx].fillAmount == 0)
                coolTimeImageArr[iconIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(iconIdx, timeCount));
            Debug.Log("MoveBack CoolTime: " + timeCount);
            yield return new WaitForSeconds(timeCount);
        }
    }

    IEnumerator BoosterCoroutine(int getRank)
    {
        character.isBoosterOn = true;
        character.SetCharacterBasicSpeedError();
        GameObject boosterParent = Instantiate(itemPrefabArr[13]);
        boosterParent.transform.SetParent(character.transform);
        boosterParent.transform.localPosition = character.transform.position + new Vector3(2.13f, 0, 0);
        boosterParent.GetComponent<Booster>().character = character.GetComponent<SpriteRenderer>();
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.Booster);

        float getBasicSpeed = (float)character.player.moveSpeed;
        float getAttackSpeed = (float)character.player.attackSpeed;
        float getAttackRange = (float)character.player.attackRange;
        float getAttackPower = (float)character.player.attackPower;

        float timeCount = 300 / getAttackSpeed;
        float duration = getAttackRange;
        float speed = getAttackPower;

        switch (getRank)
        {
            case 0:
                timeCount = 30 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[12].attackSpeedValue);
                duration = getAttackRange * (float)DataManager.Instance.consumableList.item[12].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * (float)DataManager.Instance.consumableList.item[12].attackPowerValue;
                break;
            case 1:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[27].attackSpeedValue);
                duration = getAttackRange * (float)DataManager.Instance.consumableList.item[27].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * (float)DataManager.Instance.consumableList.item[27].attackPowerValue;
                break;
            case 2:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[42].attackSpeedValue);
                duration = getAttackRange * (float)DataManager.Instance.consumableList.item[42].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * (float)DataManager.Instance.consumableList.item[42].attackPowerValue;
                break;
            case 3:
                timeCount = 20 / (getAttackSpeed * (float)DataManager.Instance.consumableList.item[57].attackSpeedValue);
                duration = getAttackRange * (float)DataManager.Instance.consumableList.item[57].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * (float)DataManager.Instance.consumableList.item[57].attackPowerValue;
                break;
        }

        while (!NightManager.Instance.isStageEnd)
        {
            NightSFXManager.Instance.PlayAudioClip(AudioClipName.booster);
            character.Movement.SetMoveSpeed(speed);
            boosterParent.SetActive(true);
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAA");
            yield return new WaitForSeconds(duration);
            Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBB");
            character.Movement.SetMoveSpeed(getBasicSpeed);
            boosterParent.SetActive(false);

            if (coolTimeImageArr[iconIdx].fillAmount == 0)
                coolTimeImageArr[iconIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(iconIdx, timeCount));
            yield return new WaitForSeconds(timeCount);
        }
        Debug.Log("CCCCCCCCCCCCCCCCCC");
    }

    void BioSnach(int getRank)
    {
        int iconIdx = FindIconIdx((int)ItemTypes.BioSnach);
        Debug.Log(iconIdx + " iconidx");
        float getAbsorbAttackData = 1;

        switch (getRank)
        {
            case 0:
                getAbsorbAttackData = 1;
                break;
            case 1:
                getAbsorbAttackData = 2;
                break;
            case 2:
                getAbsorbAttackData = 3;
                break;
            case 3:
                getAbsorbAttackData = 5;
                break;
        }
        character.player.healByHit = getAbsorbAttackData;
    }

    void InterceptDroneCoroutine(int getRank)
    {
        //아이콘 인덱스 찾기
        int iconIdx = FindIconIdx((int)ItemTypes.InterceptDrone);

        GameObject interceptDroneParent = Instantiate(itemPrefabArr[15]);
        interceptDroneParent.transform.SetParent(character.transform);
        interceptDroneParent.transform.localPosition = character.transform.position;
        interceptDroneParent.GetComponent<InterceptDrone>().character = character;
        interceptDroneParent.GetComponent<InterceptDrone>().nightManager = NightManager.Instance;
        interceptDroneParent.GetComponent<InterceptDrone>().nightSFXManager = NightSFXManager.Instance;
        interceptDroneParent.GetComponent<InterceptDrone>().coolTimeImage = coolTimeImageArr[iconIdx];
        interceptDroneParent.GetComponent<InterceptDrone>().SetItemRank(getRank);

        float getAttackSpeed = (float)character.player.attackSpeed;
        float getAttackRange = (float)character.player.attackRange;

        interceptDroneParent.GetComponent<InterceptDrone>().SetInterceptDrone(getAttackRange, getAttackSpeed);
    }

    int FindIconIdx(int itemIdx)
    {
        for (int i = 0; i < itemIdxArr.Length; i++)
        {
            if (itemIdx == itemIdxArr[i])
                return i;
        }

        //못찾으면 -1
        return -1;
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
