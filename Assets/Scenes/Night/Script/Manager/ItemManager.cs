using System.Collections;
using System.Collections.Generic;
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
                    DisasterDrone(item);
                    break;
                case ItemTypes.MultiSlash:
                    StartCoroutine(MultiSlashCoroutine(item));
                    break;
                case ItemTypes.RailPiercer:
                    RailPiercer(item);
                    break;
                case ItemTypes.FirstAde:
                    StartCoroutine(FirstAdeCoroutine(item));
                    break;
                case ItemTypes.Barrior:
                    StartCoroutine(BarriorCoroutine(item));
                    break;
                case ItemTypes.HologramTrick:
                    StartCoroutine(HologramTrickCoroutine(item));
                    break;
                case ItemTypes.AntiPhenet:
                    AntiPhenet(item);
                    break;
                case ItemTypes.RegenerationArmor:
                    RegenerationArmor(item);
                    break;
                case ItemTypes.GravityBind:
                    GravityBind(item);
                    break;
                case ItemTypes.MoveBack:
                    StartCoroutine(MoveBack(item));
                    break;
                case ItemTypes.Booster:
                    StartCoroutine(BoosterCoroutine(item));
                    break;
                case ItemTypes.BioSnach:
                    BioSnach(item);
                    break;
                case ItemTypes.InterceptDrone:
                    InterceptDroneCoroutine(item);
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









    void DisasterDrone(Consumable item)
    {
        GameObject disasterDroneParent = Instantiate(itemPrefabArr[3]);
        disasterDroneParent.transform.SetParent(character.transform);
        disasterDroneParent.transform.localPosition = character.transform.position;
        disasterDroneParent.GetComponent<DisasterDrone>().character = character;
        disasterDroneParent.GetComponent<DisasterDrone>().nightManager = NightManager.Instance;
        disasterDroneParent.GetComponent<DisasterDrone>().SetItemRank(item.rank);
    }




    IEnumerator MultiSlashCoroutine(Consumable item)
    {

        //랭크에 따라 다른 작업 추가
        float slashTime = 6;
        float attackPowerRate = (float)DataManager.Instance.consumableList.item[3].attackPowerValue;
        float slashAttackPower = (float)character.player.attackPower * attackPowerRate;
        float attackRange = (float)character.player.attackRange;

        switch (item.rank)
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
            ShootSwordAura(slashAttackPower, attackRange, item.rank);

            if (coolTimeImageArr[item.imgIdx].fillAmount == 0)
                coolTimeImageArr[item.imgIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(item.imgIdx, slashTime));

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

    void RailPiercer(Consumable item)
    {
        GameObject railPiercerParent = Instantiate(itemPrefabArr[5]);
        railPiercerParent.transform.SetParent(characterParent.transform);
        RailPiercer railPiercerScript = railPiercerParent.GetComponent<RailPiercer>();

        railPiercerScript.character = character;
        railPiercerScript.nightManager = NightManager.Instance;
        railPiercerScript.nightSFXManager = NightSFXManager.Instance;

        railPiercerScript.coolTimeImage = coolTimeImageArr[item.imgIdx];
        railPiercerScript.SetItemRank(item.rank);

        railPiercerScript.SetRailPiercerPos();
        railPiercerScript.ShootRailPiercer();
    }

    IEnumerator FirstAdeCoroutine(Consumable item)
    {

        GameObject firstAdeParent = Instantiate(itemPrefabArr[6]);
        firstAdeParent.transform.SetParent(character.transform);
        firstAdeParent.transform.localPosition = character.transform.position;
        firstAdeParent.SetActive(false);

        double nowHp = GameManager.Instance.player.curHp;
        double firstAdeHp = character.player.maxHp * 0.4f;
        float healHp = (float)character.player.attackPower;
        int coolTime = 30;

        switch (item.rank)
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
            coolTimeImageArr[item.imgIdx].fillAmount = 1;
            character.HealHp(healHp, firstAdeParent);

            NightSFXManager.Instance.PlayAudioClip(AudioClipName.firstAde);
            StartCoroutine(SetCooltimeCoroutine(item.imgIdx, coolTime));
            yield return new WaitForSeconds(coolTime);
        }
    }

    IEnumerator BarriorCoroutine(Consumable item)
    {

        GameObject barriorParent = Instantiate(itemPrefabArr[7]);
        barriorParent.transform.SetParent(character.transform);
        barriorParent.transform.position = character.transform.position;
        Barrior barriorScript = barriorParent.GetComponent<Barrior>();

        float characterAttackSpeed = (float)character.player.attackSpeed;
        float characterAttackPower = (float)character.player.attackPower;
        float shieldPoint = characterAttackPower;

        double timeCount = 50 / characterAttackSpeed;

        switch (item.rank)
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
            if (coolTimeImageArr[item.imgIdx].fillAmount == 0)
                coolTimeImageArr[item.imgIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(item.imgIdx, (float)timeCount));
            yield return new WaitForSeconds((float)timeCount);
        }
    }

    IEnumerator HologramTrickCoroutine(Consumable item)
    {

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

        switch (item.rank)
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

            if (coolTimeImageArr[item.imgIdx].fillAmount == 0)
                coolTimeImageArr[item.imgIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(item.imgIdx, timeCount));
            yield return new WaitForSeconds(timeCount);
        }

        character.isHologramAnimate = false;
    }

    void AntiPhenet(Consumable item)
    {
        character.isAntiPhenetOn = true;

        switch (item.rank)
        {
            case 0:
                character.SetAntiPhenetData(DataManager.Instance.consumableList.item[8].attackPowerValue);
                break;
            case 1:
                character.SetAntiPhenetData(DataManager.Instance.consumableList.item[23].attackPowerValue);
                break;
            case 2:
                character.SetAntiPhenetData(DataManager.Instance.consumableList.item[38].attackPowerValue);
                break;
            case 3:
                character.SetAntiPhenetData(DataManager.Instance.consumableList.item[53].attackPowerValue);
                break;
        }
    }

    void RegenerationArmor(Consumable item)
    {

        float addHp = character.player.attackPower;
        float addHpRegen = character.player.attackRange;

        switch (item.rank)
        {
            case 0:
                addHp *= DataManager.Instance.consumableList.item[9].attackPowerValue;
                addHpRegen *= DataManager.Instance.consumableList.item[9].attackRangeValue;
                Debug.Log(addHpRegen);
                break;
            case 1:
                addHp *= DataManager.Instance.consumableList.item[24].attackPowerValue;
                addHpRegen *= DataManager.Instance.consumableList.item[24].attackRangeValue;
                break;
            case 2:
                addHp *= DataManager.Instance.consumableList.item[39].attackPowerValue;
                addHpRegen *= DataManager.Instance.consumableList.item[39].attackRangeValue;
                break;
            case 3:
                addHp *= DataManager.Instance.consumableList.item[54].attackPowerValue;
                addHpRegen *= DataManager.Instance.consumableList.item[54].attackRangeValue;
                break;
        }

        character.Health.HealToMaxHp();
        character.player.hpRegen = addHpRegen;
    }

    void GravityBind(Consumable item)
    {
        GameObject gravityBindParent = Instantiate(itemPrefabArr[11]);
        gravityBindParent.transform.SetParent(characterParent.transform);
        gravityBindParent.transform.localPosition = character.transform.position;
        gravityBindParent.transform.GetChild(0).GetComponent<GravityBind>().character = character;
        gravityBindParent.transform.GetChild(0).GetComponent<GravityBind>().nightManager = NightManager.Instance;
        gravityBindParent.transform.GetChild(0).GetComponent<GravityBind>().SetItemRank(item.rank);
    }

    IEnumerator MoveBack(Consumable item)
    {

        float getAttackSpeed = (float)character.player.attackSpeed;
        float timeCount = 200 / getAttackSpeed;

        switch (item.rank)
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

            if (coolTimeImageArr[item.imgIdx].fillAmount == 0)
                coolTimeImageArr[item.imgIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(item.imgIdx, timeCount));
            Logger.Log("MoveBack CoolTime: " + timeCount);
            yield return new WaitForSeconds(timeCount);
        }
    }

    IEnumerator BoosterCoroutine(Consumable item)
    {
        character.isBoosterOn = true;
        character.SetCharacterBasicSpeedError();
        GameObject boosterParent = Instantiate(itemPrefabArr[13]);
        boosterParent.transform.SetParent(character.transform);
        boosterParent.transform.localPosition = character.transform.position + new Vector3(2.13f, 0, 0);
        boosterParent.GetComponent<Booster>().character = character.GetComponent<SpriteRenderer>();


        float getBasicSpeed = character.player.moveSpeed;
        float getAttackSpeed = character.player.attackSpeed;
        float getAttackRange = character.player.attackRange;
        float getAttackPower = character.player.attackPower;

        float timeCount = 300 / getAttackSpeed;
        float duration = getAttackRange;
        float speed = getAttackPower;

        switch (item.rank)
        {
            case 0:
                timeCount = 30 / (getAttackSpeed * DataManager.Instance.consumableList.item[12].attackSpeedValue);
                duration = getAttackRange * DataManager.Instance.consumableList.item[12].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * DataManager.Instance.consumableList.item[12].attackPowerValue;
                break;
            case 1:
                timeCount = 20 / (getAttackSpeed * DataManager.Instance.consumableList.item[27].attackSpeedValue);
                duration = getAttackRange * DataManager.Instance.consumableList.item[27].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * DataManager.Instance.consumableList.item[27].attackPowerValue;
                break;
            case 2:
                timeCount = 20 / (getAttackSpeed * DataManager.Instance.consumableList.item[42].attackSpeedValue);
                duration = getAttackRange * DataManager.Instance.consumableList.item[42].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * DataManager.Instance.consumableList.item[42].attackPowerValue;
                break;
            case 3:
                timeCount = 20 / (getAttackSpeed * DataManager.Instance.consumableList.item[57].attackSpeedValue);
                duration = getAttackRange * DataManager.Instance.consumableList.item[57].attackRangeValue;
                speed = getBasicSpeed + getAttackPower * DataManager.Instance.consumableList.item[57].attackPowerValue;
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

            if (coolTimeImageArr[item.imgIdx].fillAmount == 0)
                coolTimeImageArr[item.imgIdx].fillAmount = 1;
            StartCoroutine(SetCooltimeCoroutine(item.imgIdx, timeCount));
            yield return new WaitForSeconds(timeCount);
        }
        Debug.Log("CCCCCCCCCCCCCCCCCC");
    }

    void BioSnach(Consumable item)
    {
        float getAbsorbAttackData = 1;

        switch (item.rank)
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

    void InterceptDroneCoroutine(Consumable item)
    {

        GameObject interceptDroneParent = Instantiate(itemPrefabArr[15]);
        interceptDroneParent.transform.SetParent(character.transform);
        interceptDroneParent.transform.localPosition = character.transform.position;
        interceptDroneParent.GetComponent<InterceptDrone>().character = character;
        interceptDroneParent.GetComponent<InterceptDrone>().nightManager = NightManager.Instance;
        interceptDroneParent.GetComponent<InterceptDrone>().nightSFXManager = NightSFXManager.Instance;
        interceptDroneParent.GetComponent<InterceptDrone>().coolTimeImage = coolTimeImageArr[item.imgIdx];
        interceptDroneParent.GetComponent<InterceptDrone>().SetItemRank(item.rank);

        float getAttackSpeed = character.player.attackSpeed;
        float getAttackRange = character.player.attackRange;

        interceptDroneParent.GetComponent<InterceptDrone>().SetInterceptDrone(getAttackRange, getAttackSpeed);
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
