using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;         // инстанс (объект одиночка ?)

    [Header("Ссылки")]
    public Player player;                       // ссылка на игрока
    public Terrain terrain;                     // ссылка на террейн
    public PostProcessManager postProcessManager;
    [HideInInspector] public DiffManager diffManager;             // ссылка на диффменеджер

    // Enemy spawner
    [HideInInspector] public int enemyCount = 0;                  // счетчик зомби
    [HideInInspector] public bool inBuilding = false;             // для изменения сферы прозрачности в здании 

    // Quest    
    [HideInInspector] public bool questAmmo = false;              // для выпадения патронов на все оружия
    [HideInInspector] public bool quest1 = false;                 // для выполненых квестов 
    [HideInInspector] public bool quest2 = false;
    [HideInInspector] public bool quest3 = false;
    [HideInInspector] public bool quest1Compl = false;
    [HideInInspector] public bool quest2Compl = false;

    [HideInInspector]
    public bool quest3Compl = false;
    [HideInInspector]
    public bool questFinish = false;            // собраны 3 предмета
    
    public bool questCompl = false;             // тру когда полностью выполнен квест и вызван вертолёт
    
    public bool final = false;                  // для финального ивента (обычные спавнеры останавливаются)

    [HideInInspector]
    public bool lightsOff = false;              // для выключения света в городе 

    [HideInInspector]
    public int weakZombiesChanse;               // для изменения процента появления слабых зомби (пока не используется)

    // Диалог
    public DialogueTrigger dialogueTrig;        // диалог (для текста в начале)

    // Текущая миссия
    public string mission;                      // текст миссии на экране

    public GameObject spawnPointsGameobject;    // для управления спавнерами (ссылка на группу спавнеров)
    private EnemySpawnPoint[] spawnPoints;      // тоже 
    public Transform[] transformSpawnPoints;    // для точек назначения НПС

    public int startDiffDelay_1;                // начальная задержка перед спауном зомби
    public int startDiffDelay_2;                // начальная задержка перед спауном зомби
    public int startDiffDelay_3;                // начальная задержка перед спауном зомби

    public int finalDelay = 60;                  // задержка перед завершением финального ивента (сколько он длится)
    public bool pultActive = false;              // активация пульта

    public GameObject finalSpotLamp;            // прожектор вертолёта 

    int i = 0;                                  // счетчик для сложности

    bool pause = false;                          // для паузы
    //bool slowMo = false;                        // для слоумоушен
        
    public bool playerDead = false;             // заряд для диалога при поражении

    public bool playerStop = false;             // для обездвижевания
        
    public GameObject bars;                     // для включения баров

    [Header("Засветление")]
    [Tooltip("Начинает засветлять экран")]
    public bool postProcessFinal = false;       // для "засветления" в финале

    public GameObject tempCam;                  // камера при старте для карты
    public GameObject tempLight;                // свет при старте для карты

    public GameObject canvasMap;                // Карта
    bool mapActive = false;                     // вкл/выкл карту

    public GameObject deathMenu;                // меню при поражении
    public GameObject pauseMenu;                // меню паузы
    public bool startCinema = true;             // для начального ролика
    public GameObject blackScreen;              // черный экран для начала игры

    public RaycastWeapon weaponPrefab;          // префаб пистолета, чтобы экипировать в начале
    public RaycastWeapon weaponPrefabAxe;       // префаб топора, чтобы экипировать в начале

    public GameObject mapPlayerIcon;            // иконка игрока на карте

    public int enemyKilledCount = 0;            // счетчик убийства зомби (используется для сложности)
    

    public bool test;                           // для режима теста
    //public bool mainScene;               // для основной сцены

    public bool noKillSphere;                   // не убивать зомби при их выходе из спавнящей сферы
    public bool zombieFreeWalk;                 // зомби идут в рандомном направлении
    public int zombieToKillWaveGM;              // кол-во зомби для режима выживания
    public bool survZombie;                     // зомби для режима выживания (триггер = 1000)



    public bool enemyAllAmmo;                   // для всех видов патронов (выживание)

    public GameObject[] menus;                  // все меню

    public NPC npc;                             // ссылка на НПС

    public int delayUp;                         // задержка когда встаёт (для меин сцены 11,5 сек)

    public Transform[] pointsNPC;               // точки для спауна НПС
    public bool mutation;                       // мутация
    public int mutationNumber = 1;              // номер мутации
    public GameObject textMutation;             // бар мутации
    public bool postProcessStart;               // старт оттемнения

    public GameObject[] closes;                 // одежда
    int personTypeNumber;                       // для выбора одежды персонажа
    public bool camFollowPlayer;                // следовать камере за игроком или нет
    public GameObject startScene;               // стартовая сцена
    public GameObject loadScreen;               // экран загрузки
    //public AudioListenerPlayer listner;


    // Статистика
    public int enemyKilledStatistic;            // счетчик убийства зомби (статистика)
    public int rayCastsStatistic;               // счетчик выстрелов
    public int granateStatistic;                // счетчик гранат
    public int hpBoxStatistic;                  // счетчик аптечек
    public int axeAttackStatistic;              // счетчик атак топором
    public int handAttackStatistic;             // счетчик атак врукопашную
     



    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\


    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
/*            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            Destroy(eventSys);*/

            return;
        }
        // присваем instance (?) этому обьекту и по ивенту загрузки запускаем функцию загрузки
        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;        
    }        

    public void Start()
    {
        diffManager = GetComponent<DiffManager>();
        dialogueTrig = GetComponent<DialogueTrigger>();                                     // Ссылка на диалог
        spawnPoints = spawnPointsGameobject.GetComponentsInChildren<EnemySpawnPoint>();
        transformSpawnPoints = spawnPointsGameobject.GetComponentsInChildren<Transform>();        


/*        if (mainScene)
            StartCoroutine(StartDiffCor());                                                 // начальная сложность, задержка*/
        if (test)
            StartCoroutine(ActionStart());
        if (!test)
        {
            playerStop = true;                                                              // забираем контроль
            //StartCoroutine(DialogePause(delayUp));                                          // начальный диалог 
        }
        
        tempCam.SetActive(true);                                                            // для карты
        tempLight.SetActive(true);                                                          // для карты
        terrain.treeDistance = 300;
        StartCoroutine(TempCamDelay());                                                     // для карты
        mapPlayerIcon.SetActive(true);                                                      // включаем иконку игрока на карте
        //loadScreen.SetActive(true);             // загрузочный экран

        //PauseWithDelay();
        //Pause();
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------\\




    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && diffManager.waveN < 21)
        {
            diffManager.waveN += 1;
            diffManager.waveNumber += 1;
            diffManager.start = true;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            player.ammoPack.souls += 100;
        }

/*
        if (Input.GetKeyDown(KeyCode.Escape) && !startCinema)
        {
            foreach (GameObject menu in menus)
            {
                menu.SetActive(false);                
            }
            if (npc.magazineOpen)
                npc.CloseMagazine();
            player.aiming = true;
            Time.timeScale = 1f;
        }  */    


        // Пауза
        if (Input.GetKeyDown(KeyCode.Escape) && !playerDead && !startCinema)
        {            
            if (pause && !npc.magazineOpen)
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                playerStop = false;
                pause = false;
                player.aiming = true;
                foreach (GameObject menu in menus)
                {
                    menu.SetActive(false);
                }
            }            
            else if (npc.magazineOpen)
            {
                npc.CloseMagazine();
            }
            else
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                playerStop = true;
                pause = true;
            }
        }


        // Слоумоушион нах
        /*        if (Input.GetKeyDown(KeyCode.U))
                {
                    slowMo = !slowMo;
                    if (slowMo)
                        Time.timeScale = 0.3f;
                    if (!slowMo)
                        Time.timeScale = 1f;
                }  */


        // Карта
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            if (!player.isAlive || playerStop || startCinema)
            {
                return;
            }

            mapActive = !mapActive;

            if (mapActive)
            {
                canvasMap.SetActive(true);
                //Time.timeScale = 0f;
            }
            if (!mapActive)
            {
                canvasMap.SetActive(false);
                //Time.timeScale = 1f;
            }
        }


        // Квест
        if (quest1 && !quest1Compl)             // после выполнения квеста
        {
            SetDifficulty();                    // добавляем сложность
            quest1Compl = true;                 // этот квест выполнен

            // в целях поменать на +
        }
        if (quest2 && !quest2Compl)
        {
            SetDifficulty();
            quest2Compl = true;

            // в целях поменать на +
        }
        if (quest3 && !quest3Compl)
        {
            SetDifficulty();
            quest3Compl = true;
        }
        if (quest1 && quest2 && quest3)
        {
            //SetDifficulty();
            questFinish = true;         // собраны 3 предмета             
        }


        // Если проиграл
        if (!player.isAlive && !playerDead)
        {
            StartCoroutine(DialogePauseDeath());
            playerDead = true;
        }
    }



    //-------------------------------------------------Начальные ролик и настройки--------------------------------------------------------------------------------------------------\\

    IEnumerator TempCamDelay()                                  // задержка для выключения камеры карты
    {
        yield return new WaitForSeconds(5f);
        loadScreen.SetActive(false);
        tempCam.SetActive(false);
        tempLight.SetActive(false);
        terrain.treeDistance = 30;
    }



    // Выбор персонажа
    public void SetCharacter(int numberCharacter)
    {
        switch (numberCharacter)
        {
            case 1:
                closes[0].SetActive(true);                  // выбор одежды
                personTypeNumber = 4;                       // выбор диалога
                player.maxHealth = 150;
                player.currentHealth = 150;
                player.ammoPack.GiveWeapon("Pistol");
                player.ammoPack.allAmmo_9 = 300;
                player.ammoPack.GiveWeapon("AR");
                player.ammoPack.allAmmo_5_56 = 200;
                player.ammoPack.GiveArmor("1");
                player.ammoPack.allAmmo_7_62 = 300;
                player.ammoPack.granate = 3;
                player.ammoPack.HPBox = 3;
                player.agent.Warp(new Vector3(73.8f, 0, -113));
                break;

            case 2:
                closes[1].SetActive(true);
                personTypeNumber = 5;
                player.xSpeed = 7;
                player.ySpeed = 7;
                player.maxSpeed = 7;
                player.cooldownSlow = 0.1f;                 // быстрее выходит из замедления
                player.activeWeapon.lifeSteal = true;
                player.ammoPack.GetAxe();
                player.ammoPack.GiveWeapon("Shotgun");
                player.ammoPack.allAmmo_0_12 = 56;
                player.activeWeapon.lifeSteal = true;
                player.ammoPack.HPBox = 1;
                player.agent.Warp(new Vector3(34.5f, 0, -257f));
                break;

            case 3:
                closes[2].SetActive(true);
                personTypeNumber = 6;
                player.ammoPack.GiveWeapon("Revolver");
                player.ammoPack.allAmmo_0_357 = 96;
                player.ammoPack.granate = 1;
                player.ammoPack.HPBox = 1;
                player.activeWeapon.rigController.speed = 2;
                player.agent.Warp(new Vector3(290, 0, -170));
                break;

            case 4:
                closes[3].SetActive(true);
                personTypeNumber = 7;
                player.anim.SetTrigger("down");
                player.currentHealth = 49;
                player.lightOn = false;
                player.agent.Warp(new Vector3(302, 0, -311));
                break;
        }
        player.ammoPack.souls = 0;
        startScene.SetActive(false);                                    // убираем всю стартовую сцену
        enemyCount = 0;                                                 // сбрасываем счётчик зомби на карте
        camFollowPlayer = true;                                         // камера следует за игроком
        blackScreen.SetActive(true);                                    // включаем черный экран (на ~полсекунды)
        postProcessManager.SetGammaMinus();                             // затемняем полностью
        if (!test)
            diffManager.start = true;                                   // запускаем уровень
        //UnPause();
        StartCoroutine(DialogePause(delayUp, personTypeNumber));        // запускаем начальный диалог с задержкой и номером персонажа
        //StartCoroutine(ActionStart());
    }

    IEnumerator DialogePause(int delay, int personNumber)       // начальный ролик и настройки
    {
        yield return new WaitForSeconds(1f);                    // задержка для черного экрана
        blackScreen.SetActive(false);                           // включаем черный экран (на ~полсекунды)
        postProcessStart = true;                                // начинаем оттенение        
        yield return new WaitForSeconds(delay);                 // задержка пока персонаж встаёт

        dialogueTrig.TriggerDialogue(personNumber);             // показываем диалог                                           
        PauseWithDelay();  
        yield return new WaitForSeconds(1f);

        playerStop = false;                                     // отдаём контроль
        startCinema = false;                                    // ролик завершён

        dialogueTrig.TriggerDialogue(0);                        // показываем диалог
        PauseWithDelay();                                       // паузу, чтобы было время почитать
        StartCoroutine(ActionStart());                          

        //player.activeWeapon.EquipActiveStart();
    }

    IEnumerator ActionStart()
    {
        yield return new WaitForSeconds(1f);        
        playerStop = false;                                     // отдаём контроль (в тестовом режиме)
        startCinema = false;                                    // ролик завершён
        postProcessStart = true;                                // начинаем оттенение 
        blackScreen.SetActive(false);                           
        bars.SetActive(true);                                   // показываем бары
        //npc.OpenMagazine();                                     // открываем магазин для начального закупа

        /*RaycastWeapon newWeapon = Instantiate(weaponPrefab);        
        player.activeWeapon.GetWeaponUp(newWeapon);*/
      

    }


    public void HidePauseMenu()                                 // для кнопки "продолжить" в меню паузы
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        playerStop = false;
        pause = false;
    }



    //--------------------------------------- Управление сложностью --------------------------------------------------\\


    IEnumerator StartDiffCor()                      // постепенное увеличение сложности
    {
        //Debug.Log("Cor!");
        yield return new WaitForSeconds(startDiffDelay_1);
        SetDifficultyStart();
        yield return new WaitForSeconds(startDiffDelay_2);
        SetDifficulty();
        yield return new WaitForSeconds(startDiffDelay_3);
        SetDifficulty();
    }


    public void SetDifficultyStart()                // установление начальной сложности 
    {
        //Debug.Log("Set!");
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            
                spawnPoint.maxZombie = 9;
            
                spawnPoint.enemyNumberSpawn = -1;
            
                spawnPoint.cooldown = 12;
        }
    }


    public void SetDifficulty()                     // увеличение сложности 
    {
        i++;
        //Debug.Log("Set!");
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {            
            spawnPoint.maxZombie += 7;

            if (i < 2)
                spawnPoint.enemyNumberSpawn += 1;
            
            spawnPoint.cooldown -= 1;
        }
    }

    public void MaxDifficulty()                     // временное увеличение сложности на ивентах
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.maxZombie += 10;
            spawnPoint.enemyNumberSpawn += 2;
            spawnPoint.cooldown -= 2;
        }
        StartCoroutine(MaxDiffOff());
    }

    IEnumerator MaxDiffOff()                         // отключаем повышение сложности через .. секунд
    {
        yield return new WaitForSeconds(60);
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {
            spawnPoint.maxZombie -= 10;
            spawnPoint.enemyNumberSpawn -= 2;
            spawnPoint.cooldown += 2;
        }
    }

    public void SetFinalDifficulty()                // финальная сложность
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 50;

            spawnPoint.enemyNumberSpawn = 2;

            spawnPoint.cooldown = 8;
        }
    }

    public void SetNullDifficulty()                 // сложность обычных спавнеров для финала
    {
        foreach (EnemySpawnPoint spawnPoint in spawnPoints)
        {

            spawnPoint.maxZombie = 0;

            spawnPoint.enemyNumberSpawn = 0;

            spawnPoint.cooldown = 1000;
        }
    }




    public void SetDifficultyNumber(int diffNumber)            // сложность для режима выживания
    {        
        switch (diffNumber)
        {
            case 0:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 0;
                    spawnPoint.enemyNumberSpawn = 0;
                    spawnPoint.cooldown = 10;
                    spawnPoint.mediumZombieChanse = 0;
                    spawnPoint.strongZombieChanse = 0;
                    zombieToKillWaveGM = 999;

                    //diffManager.positionNPC = pointsNPC[0].position;
                }
                break;

            case 1:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 20;
                    spawnPoint.enemyNumberSpawn = 1;
                    spawnPoint.cooldown = 6;
                    spawnPoint.mediumZombieChanse = 0;
                    spawnPoint.strongZombieChanse = 0;
                    spawnPoint.darkZombieChanse = 0;
                    zombieToKillWaveGM = 17;
                    diffManager.positionNPC = pointsNPC[1].position;
                }
                break;

            case 2:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 26;
                    spawnPoint.enemyNumberSpawn = 1;
                    spawnPoint.cooldown = 6;
                    spawnPoint.mediumZombieChanse = 10;
                    spawnPoint.strongZombieChanse = 0;
                    zombieToKillWaveGM = 27;
                    diffManager.positionNPC = pointsNPC[2].position;
                }
                break;

            case 3:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 30;
                    spawnPoint.enemyNumberSpawn = 1;
                    spawnPoint.cooldown = 5;
                    spawnPoint.mediumZombieChanse = 13;
                    spawnPoint.strongZombieChanse = 3;
                    spawnPoint.darkZombieChanse = 0;
                    zombieToKillWaveGM = 37;
                    diffManager.positionNPC = pointsNPC[3].position;
                }
                break;

            case 4:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 31;
                    spawnPoint.enemyNumberSpawn = 1;
                    spawnPoint.cooldown = 5;
                    spawnPoint.mediumZombieChanse = 15;
                    spawnPoint.strongZombieChanse = 5;
                    spawnPoint.darkZombieChanse = 0;
                    zombieToKillWaveGM = 40;
                    diffManager.positionNPC = pointsNPC[4].position;
                }
                break;

            case 5:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 32;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 0;
                    spawnPoint.strongZombieChanse = 0;
                    spawnPoint.darkZombieChanse = 10;
                    zombieToKillWaveGM = 50;
                    diffManager.positionNPC = pointsNPC[5].position;
                }
                break;

            case 6:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 33;
                    spawnPoint.enemyNumberSpawn = 3;
                    spawnPoint.cooldown = 3;
                    spawnPoint.mediumZombieChanse = 20;
                    spawnPoint.strongZombieChanse = 7;
                    spawnPoint.darkZombieChanse = 5;
                    zombieToKillWaveGM = 50;
                    diffManager.positionNPC = pointsNPC[6].position;
                }
                break;

            case 7:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 34;
                    spawnPoint.enemyNumberSpawn = 3;
                    spawnPoint.cooldown = 3;
                    spawnPoint.mediumZombieChanse = 20;
                    spawnPoint.strongZombieChanse = 10;
                    spawnPoint.darkZombieChanse = 6;
                    zombieToKillWaveGM = 50;
                    diffManager.positionNPC = pointsNPC[7].position;

                    //diffManager.positionNPC = new Vector3(181, 0, -205);
                }
                break;



            case 8:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 35;
                    spawnPoint.enemyNumberSpawn = 3;
                    spawnPoint.cooldown = 3;
                    spawnPoint.mediumZombieChanse = 22;
                    spawnPoint.strongZombieChanse = 12;
                    spawnPoint.darkZombieChanse = 8;
                    zombieToKillWaveGM = 61;
                    diffManager.positionNPC = pointsNPC[8].position;

                    //diffManager.positionNPC = new Vector3(257, 0, -273);
                }
                break;

            case 9:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 36;
                    spawnPoint.enemyNumberSpawn = 3;
                    spawnPoint.cooldown = 3;
                    spawnPoint.mediumZombieChanse = 23;
                    spawnPoint.strongZombieChanse = 15;
                    spawnPoint.darkZombieChanse = 10;
                    zombieToKillWaveGM = 62;
                    diffManager.positionNPC = pointsNPC[9].position;

                    //diffManager.positionNPC = new Vector3(309, 0, -322);
                }
                break;

            case 10:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 20;
                    spawnPoint.enemyNumberSpawn = 3;
                    spawnPoint.cooldown = 3;
                    spawnPoint.mediumZombieChanse = 25;
                    spawnPoint.strongZombieChanse = 5;
                    spawnPoint.darkZombieChanse = 100;
                    zombieToKillWaveGM = 30;
                    diffManager.positionNPC = pointsNPC[10].position;

                    //diffManager.positionNPC = new Vector3(347, 0, -174);
                }
                break;

            case 11:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 25;
                    spawnPoint.strongZombieChanse = 15;
                    spawnPoint.darkZombieChanse = 10;
                    zombieToKillWaveGM = 82;
                    diffManager.positionNPC = pointsNPC[11].position;

                    //diffManager.positionNPC = new Vector3(264, 0, -45);
                }
                break;

            case 12:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {

                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 20;
                    spawnPoint.strongZombieChanse = 20;
                    spawnPoint.darkZombieChanse = 10;
                    zombieToKillWaveGM = 84;
                    diffManager.positionNPC = pointsNPC[12].position;

                    //diffManager.positionNPC = new Vector3(141, 0, -73);
                }
                break;

            case 13:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 60;
                    spawnPoint.strongZombieChanse = 10;
                    spawnPoint.darkZombieChanse = 10;
                    zombieToKillWaveGM = 80;
                    diffManager.positionNPC = pointsNPC[13].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 14:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 30;
                    spawnPoint.strongZombieChanse = 20;
                    spawnPoint.darkZombieChanse = 15;
                    zombieToKillWaveGM = 90;
                    diffManager.positionNPC = pointsNPC[14].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 15:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 50;
                    spawnPoint.strongZombieChanse = 80;
                    spawnPoint.darkZombieChanse = 0;
                    zombieToKillWaveGM = 101;
                    diffManager.positionNPC = pointsNPC[15].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 16:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 35;
                    spawnPoint.strongZombieChanse = 25;
                    spawnPoint.darkZombieChanse = 25;
                    zombieToKillWaveGM = 110;
                    diffManager.positionNPC = pointsNPC[16].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 17:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 40;
                    spawnPoint.strongZombieChanse = 25;
                    spawnPoint.darkZombieChanse = 25;
                    zombieToKillWaveGM = 120;
                    diffManager.positionNPC = pointsNPC[17].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 18:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 43;
                    spawnPoint.strongZombieChanse = 27;
                    spawnPoint.darkZombieChanse = 25;
                    zombieToKillWaveGM = 131;
                    diffManager.positionNPC = pointsNPC[18].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 19:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 45;
                    spawnPoint.strongZombieChanse = 30;
                    spawnPoint.darkZombieChanse = 27;
                    zombieToKillWaveGM = 145;
                    diffManager.positionNPC = pointsNPC[19].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 20:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 47;
                    spawnPoint.strongZombieChanse = 30;
                    spawnPoint.darkZombieChanse = 30;
                    zombieToKillWaveGM = 160;
                    diffManager.positionNPC = pointsNPC[20].position;
                    
                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;


            case 21:                                                     
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 40;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 50;
                    spawnPoint.strongZombieChanse = 33;
                    spawnPoint.darkZombieChanse = 33;
                    zombieToKillWaveGM = 200;
                    diffManager.positionNPC = pointsNPC[21].position;
                    
                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;

            case 22:
                mutation = true;                                            // включаем мутацию
                mutationNumber += 1;                                        // добавляем номер мутации
                textMutation.SetActive(true);
                break;

            case 99:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 50;
                    spawnPoint.enemyNumberSpawn = 4;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 30;
                    spawnPoint.strongZombieChanse = 30;
                    spawnPoint.darkZombieChanse = 30;
                    zombieToKillWaveGM = 1000000;
                    diffManager.positionNPC = pointsNPC[20].position;

                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;

            case 999:
                foreach (EnemySpawnPoint spawnPoint in spawnPoints)
                {
                    spawnPoint.maxZombie = 50;
                    spawnPoint.enemyNumberSpawn = 5;
                    spawnPoint.cooldown = 2;
                    spawnPoint.mediumZombieChanse = 0;
                    spawnPoint.strongZombieChanse = 100;
                    spawnPoint.darkZombieChanse = 100;
                    zombieToKillWaveGM = 1000000;

                    //diffManager.positionNPC = pointsNPC[20].position;
                    //diffManager.positionNPC = new Vector3(162, 0, -170);
                }
                break;

        }
        //diffManager.positionNPC = pointsNPC[diffNumber].position;
    }




    //-----------------------------------Финальный ивент------------------------------------------------------\\



    public void FinalWave()                 // запуск финального ивента 
    {
        StartCoroutine(FinalDelay());
    }


    IEnumerator FinalDelay()
    {
        yield return new WaitForSeconds(finalDelay);            // сколько длится финальный ивент        

        //Debug.Log("Wave!");
        finalSpotLamp.SetActive(true);                          // включаем прожектор вертолёта 
        yield return new WaitForSeconds(1f);
        player.FinalWave();                                     // запускаем волну, убивающую зомби (стрельба из вертолёта)
        yield return new WaitForSeconds(15f);
        dialogueTrig.TriggerDialogue(1);
        PauseWithDelay();
        postProcessFinal = true;
    }



    //-----------------------------------При поражении-------------------------------------------------\\


    IEnumerator DialogePauseDeath()
    {
        yield return new WaitForSeconds(4f);        
        deathMenu.SetActive(true);
        //Pause();        
    }



    //------------------------------------Пауза-----------------------------------------------------\\

    public void PauseWithDelay()
    {
        StartCoroutine(PauseDelay());
    }

    IEnumerator PauseDelay()
    {
        yield return new WaitForSeconds(0.2f);
        player.aiming = false;
        playerStop = true;
        Time.timeScale = 0f;
    }

    public void Pause()
    {
        player.aiming = false;
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        if (!startCinema)
            playerStop = false;
        player.aiming = true;
        Time.timeScale = 1f;
    }


//---------------------------------------------------------------------------------------------------------------------------------------------------------\\

    /*
     * 
     * 
     * 
     */

    // On Scene Loaded

    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
       /* player.transform.position = GameObject.Find("SpawnPoint").transform.position;*/
    }


    // Death Menu and Respawn
    public void Respawn()
    {
/*        deathMenuAnim.SetTrigger("Hide");
        SceneManager.LoadScene("Main");
        player.Respawn();*/
    }

    public void SaveState()
    {
/*        string s = "";

        s += "0" + "|";
        s += pesos.ToString() + "|";
        s += experience.ToString() + "|";
        s += weapon.weaponLevel.ToString();

        // записываем строку s в PlayerPrefs
        PlayerPrefs.SetString("SaveState", s);*/
        
    }


    public void LoadState(Scene s, LoadSceneMode mode)
    {
/*        SceneManager.sceneLoaded -= LoadState;
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        // вытаскиваем строку s из PlayerPrefs 
        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        // Change player skin
        pesos = int.Parse(data[1]);

        // Experience
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());

        // Change the weapon Level
        weapon.SetWeaponLevel( int.Parse(data[3]));

        */
    }
}
