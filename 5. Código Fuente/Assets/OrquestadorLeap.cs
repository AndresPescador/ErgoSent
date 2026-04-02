/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Leap.Unity
{
    using Attributes;

    /// <summary>
    /// The LeapServiceProvider provides tracked Leap Hand data and images from the device
    /// via the Leap service running on the client machine.
    /// </summary>
    /// 

    public class Incidente
    {
        
        public DateTime DtStart { get; set; }
        public DateTime DtEnd { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Hand { get; set; }
        public int RuleId { get; set; }
        public int DurationInSeconds { get; set; }
        public int Score { get; set; }
        public int AvgDegree { get; set; }
        public IList<int> AvgDegreeArray { get; set; }
        public bool EstaActivo { get; set; }
        

        public Incidente()
        {
            DtStart = System.DateTime.Now;
            DtEnd = System.DateTime.Now;
            Date = "-1";
            StartTime = "-1";
            EndTime = "-1";
            RuleId = -1;
            DurationInSeconds = -1;
            Score = -1;
            AvgDegree = -1;
            AvgDegreeArray = new List<int>();
            EstaActivo = false;
            Hand = "-1";
            
        }

        public Incidente(DateTime r1_DtStart, DateTime r1_DtEnd, string r1_Date, string r1_StartTime, string r1_EndTime, int r1_RuleId, int r1_DurationInSeconds, int r1_Score, IList<int> r1_AvgDegree, bool r1_Fallando)
        {
            DtStart = r1_DtStart;
            DtEnd = r1_DtEnd;
            Date = r1_Date;
            StartTime = r1_StartTime;
            EndTime = r1_EndTime;
            RuleId = r1_RuleId;
            DurationInSeconds = r1_DurationInSeconds;
            Score = r1_Score;
            AvgDegreeArray = r1_AvgDegree;
            EstaActivo = r1_Fallando;
        }

        private int CalcularDuracion()
        {
            this.DurationInSeconds = (int)(this.DtEnd - this.DtStart).TotalSeconds;
            return this.DurationInSeconds;
        }

        private int CalcularGradoPromedio()
        {
            if (AvgDegreeArray.Count() == 0)
            {
                return -1;
            }
            else
            {
                this.AvgDegree = (int)Math.Round(this.AvgDegreeArray.Average());
                return this.AvgDegree;
            }
        }

        public void FinalizarIncidente()
        {
            this.EndTime = System.DateTime.Now.ToString("HH:mm:ss");
            this.DtEnd = System.DateTime.Now;
            this.EstaActivo = false;
            this.CalcularDuracion();
            this.CalcularGradoPromedio();
            this.AvgDegreeArray.Clear();
        }

        public void IniciarIncidente(int RuleId, int Score)
        {
            this.EstaActivo = true;
            this.StartTime = System.DateTime.Now.ToString("HH:mm:ss"); //Registre HoraInicio
            this.DtStart = System.DateTime.Now;
            this.Date = System.DateTime.Now.ToString("MM/dd/yyyy"); //Registre Fecha
            this.RuleId = RuleId; //Registre RuleId = 1 = Desv Horizontal
            this.Score = Score; //Registre Grado = 1
        }
    }

    public class DataStorage
    {
        //Conexion a la base de datos
        IDbConnection dbcon;
        public DataStorage()
        {
            //Crear base de datos
            string connection = "URI=file:" + Application.persistentDataPath + "/My_Database";
            Debug.Log("DB Location: " + "URI=file:" + Application.persistentDataPath + "/My_Database");
            //Inicializar conexión
            dbcon = new SqliteConnection(connection);
            CrearTablas();
            Debug.Log("Created");
        }

        
        public List<string> getUsers()
        {
            dbcon.Open();
            IDbCommand cmnd_read = dbcon.CreateCommand();
            IDataReader reader;
            string query = "SELECT Name FROM Users";
            cmnd_read.CommandText = query;
            reader = cmnd_read.ExecuteReader();

            List<string> users = new List<string>();
            while (reader.Read())
            {
                users.Add(reader[0].ToString());
            }
            dbcon.Close();

            return users;
        }

        public List<string> getIds()
        {
            dbcon.Open();
            IDbCommand cmnd_read = dbcon.CreateCommand();
            IDataReader reader;
            string query = "SELECT Id FROM Users";
            cmnd_read.CommandText = query;
            reader = cmnd_read.ExecuteReader();

            List<string> ids = new List<string>();
            while (reader.Read())
            {
                ids.Add(reader[0].ToString());
            }
            dbcon.Close();

            return ids;
        }

        public void InsertarUsuario(int UserId,string UserName)
        {
            dbcon.Open();
            IDbCommand cmnd = dbcon.CreateCommand();
            cmnd.CommandText = "INSERT INTO Users " +
                "(Id, " +
                "Name) " +
                "VALUES " +
                "( " + UserId + ", '" +
                UserName + "')";
            Debug.Log("Insertando usuario: "+cmnd.CommandText.ToString());
            cmnd.ExecuteNonQuery();
            
            dbcon.Close();
        }

        public void InsertarIncidente(Incidente incidenteAGuardar)
        {
            dbcon.Open();
            if (incidenteAGuardar.EstaActivo)
            {
                Debug.Log("No se pueden guardar incidentes activos");
                //incidenteAGuardar.FinalizarIncidente();
                // return;
            }
            else
            {
                if (incidenteAGuardar.DurationInSeconds > 0)
                {
                    IDbCommand cmnd = dbcon.CreateCommand();
                    cmnd.CommandText = "INSERT INTO Incidents " +
                        "(Date, " +
                        "StartTime, " +
                        "EndTime, " +
                        "RuleId, " +
                        "DurationInSeconds, " +
                        "Score, " +
                        "Hand, " +
                        "AvgDegree," +
                        "Username," +
                        "UserId) " +
                        "VALUES " +
                        "( '" + incidenteAGuardar.Date + "', '" +
                        incidenteAGuardar.StartTime + "','" +
                        incidenteAGuardar.EndTime + "'," +
                        incidenteAGuardar.RuleId + "," +
                        incidenteAGuardar.DurationInSeconds + "," +
                        incidenteAGuardar.Score + ", '" +
                        incidenteAGuardar.Hand + "', " +
                        incidenteAGuardar.AvgDegree + ",'"+
                        BTNScriptsLogin.selectedUserName+ "',"+
                        BTNScriptsLogin.selectedUserId + ")";
                    Debug.Log(cmnd.CommandText.ToString());
                    cmnd.ExecuteNonQuery();
                }
            }
            dbcon.Close();
        }

        protected void CrearTablas()
        {

            dbcon.Open();
            IDbCommand dbcmd, dbcmd2, dbcmd3;
            IDataReader reader, reader2, reader3;

            dbcmd = dbcon.CreateCommand();
            dbcmd2 = dbcon.CreateCommand();
            dbcmd3 = dbcon.CreateCommand();

            string q_createTable =
            @"CREATE TABLE IF NOT EXISTS Incidents (
            IncidentId    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            Date  TEXT NOT NULL,
            StartTime TEXT NOT NULL,
            EndTime   TEXT NOT NULL,
            RuleId    INTEGER NOT NULL,
            DurationInSeconds INTEGER NOT NULL,
            Score INTEGER NOT NULL,
            Hand TEXT NOT NULL,
            AvgDegree INTEGER,
            Username TEXT,
            UserId INTEGER)";


            dbcmd.CommandText = q_createTable;
            reader = dbcmd.ExecuteReader();


            string q_createTable2 =
            @"CREATE TABLE IF NOT EXISTS Rules(
            RuleId    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            RuleName  INTEGER NOT NULL,
            RuleDesc  INTEGER NOT NULL
            )";

            dbcmd2.CommandText = q_createTable2;
            reader2 = dbcmd2.ExecuteReader();
            

            string q_createTable3 =
           @"CREATE TABLE IF NOT EXISTS Users (
            Id      INTEGER NOT NULL,
            Name    TEXT NOT NULL
            )";

            dbcmd3.CommandText = q_createTable3;
            reader3 = dbcmd3.ExecuteReader();

            dbcon.Close();
        }



    }


    public class OrquestadorLeap : LeapProvider
    {

        #region Constants

        /// <summary>
        /// Converts nanoseconds to seconds.
        /// </summary>
        protected const double NS_TO_S = 1e-6;

        /// <summary>
        /// Converts seconds to nanoseconds.
        /// </summary>
        protected const double S_TO_NS = 1e6;

        /// <summary>
        /// The transform array used for late-latching.
        /// </summary>
        protected const string HAND_ARRAY_GLOBAL_NAME = "_LeapHandTransforms";

        /// <summary>
        /// The maximum number of times the provider will 
        /// attempt to reconnect to the service before giving up.
        /// </summary>
        protected const int MAX_RECONNECTION_ATTEMPTS = 5;

        /// <summary>
        /// The number of frames to wait between each
        /// reconnection attempt.
        /// </summary>
        protected const int RECONNECTION_INTERVAL = 180;


        public Sprite manoFlexionOK;
        public Sprite manoFlexionBaja;
        public Sprite manoFlexionAlta;
        public GameObject desvVerticalManoIzq;
        public GameObject desvVerticalManoDer;

        public Sprite desvHorizontalOK;
        public Sprite desvHorizontalMala;
        public GameObject desvHorizontalManoIzq;
        public GameObject desvHorizontalManoDer;

        public Sprite palmGrip;
        public Sprite fingerGrip;
        public Sprite clawGrip;
        public GameObject Grip;

        public bool modoZurdo = false;

        protected float anguloHorizontal;
        protected float anguloVertical;
        protected float puntuacionAgarre;

        protected bool hayDesviacionHorizontal = false;
        protected bool hayDesviacionVertical = false;
        protected bool hayAgarreEnGarra = false;

        protected GameObject iconoAgarre;
        protected GameObject iconoDesviacionHorizontal;
        protected GameObject iconoDesviacionVertical;

        public Text IzqVert;
        public Text IzqHoriz;
        public Text DerVert;
        public Text DerHoriz;
        public Text GripScore;

        protected DataStorage dbstorage;

        //Incidentes persistentes
        Incidente incHorizontalManoIzq = new Incidente();
        Incidente incHorizontalManoDer = new Incidente();
        Incidente incVerticalManoIzq = new Incidente();
        Incidente incVerticalManoDer = new Incidente();
        Incidente incAgarre = new Incidente();
        //Incidentes auxiliares
        Incidente incHorizontal = new Incidente();
        Incidente incVertical = new Incidente();

        ////Parametros de la lectura para Regla 1
        //DateTime R1_DtStart = System.DateTime.Now, R1_DtEnd = System.DateTime.Now;
        //string R1_Date = "", R1_StartTime = "", R1_EndTime = "";
        //int R1_RuleId = -1, R1_DurationInSeconds = -1, R1_Score = -1;
        //IList<int> R1_AvgDegree = new List<int>(); 

        ////Parametros de la lectura para Regla 2
        //DateTime R2_DtStart = System.DateTime.Now, R2_DtEnd = System.DateTime.Now;
        //string R2_Date = "", R2_StartTime = "", R2_EndTime = "";
        //int R2_RuleId = -1, R2_DurationInSeconds = -1, R2_Score = -1;
        //IList<int> R2_AvgDegree = new List<int>();

        ////Parametros de la lectura para Regla 3
        //DateTime R3_DtStart = System.DateTime.Now, R3_DtEnd = System.DateTime.Now;
        //string R3_Date = "", R3_StartTime = "", R3_EndTime = "";
        //int R3_RuleId = -1, R3_DurationInSeconds = -1, R3_Score = -1;
        //IList<int> R3_AvgDegree = new List<int>();

        //

        #endregion

        #region Inspector

        public enum FrameOptimizationMode
        {
            None,
            ReuseUpdateForPhysics,
            ReusePhysicsForUpdate,
        }
        [Tooltip("When enabled, the provider will only calculate one leap frame instead of two.")]
        [SerializeField]
        protected FrameOptimizationMode _frameOptimization = FrameOptimizationMode.None;

        public enum PhysicsExtrapolationMode
        {
            None,
            Auto,
            Manual
        }
        [Tooltip("The mode to use when extrapolating physics.\n" +
                 " None - No extrapolation is used at all.\n" +
                 " Auto - Extrapolation is chosen based on the fixed timestep.\n" +
                 " Manual - Extrapolation time is chosen manually by the user.")]
        [SerializeField]
        protected PhysicsExtrapolationMode _physicsExtrapolation = PhysicsExtrapolationMode.Auto;

        [Tooltip("The amount of time (in seconds) to extrapolate the physics data by.")]
        [SerializeField]
        protected float _physicsExtrapolationTime = 1.0f / 90.0f;

#if UNITY_2017_3_OR_NEWER
        [Tooltip("When checked, profiling data from the LeapCSharp worker thread will be used to populate the UnityProfiler.")]
        [EditTimeOnly]
#else
    [Tooltip("Worker thread profiling requires a Unity version of 2017.3 or greater.")]
    [Disable]
#endif
        [SerializeField]
        protected bool _workerThreadProfiling = false;

        #endregion

        #region Internal Settings & Memory
        protected bool _useInterpolation = true;

        // Extrapolate on Android to compensate for the latency introduced by its graphics
        // pipeline.
#if UNITY_ANDROID && !UNITY_EDITOR
    protected int ExtrapolationAmount = 15;
    protected int BounceAmount = 70;
#else
        protected int ExtrapolationAmount = 0;
        protected int BounceAmount = 0;
#endif

        protected Controller _leapController;
        protected bool _isDestroyed;

        protected SmoothedFloat _fixedOffset = new SmoothedFloat();
        protected SmoothedFloat _smoothedTrackingLatency = new SmoothedFloat();
        protected long _unityToLeapOffset;

        protected Frame _untransformedUpdateFrame;
        protected Frame _transformedUpdateFrame;
        protected Frame _untransformedFixedFrame;
        protected Frame _transformedFixedFrame;

        #endregion

        #region Edit-time Frame Data

        private Action<Device> _onDeviceSafe;
        /// <summary>
        /// A utility event to get a callback whenever a new device is connected to the service.
        /// This callback will ALSO trigger a callback upon subscription if a device is already
        /// connected.
        /// 
        /// For situations with multiple devices OnDeviceSafe will be dispatched once for each device.
        /// </summary>
        public event Action<Device> OnDeviceSafe
        {
            add
            {
                if (_leapController != null && _leapController.IsConnected)
                {
                    foreach (var device in _leapController.Devices)
                    {
                        value(device);
                    }
                }
                _onDeviceSafe += value;
            }
            remove
            {
                _onDeviceSafe -= value;
            }
        }

#if UNITY_EDITOR
        private Frame _backingUntransformedEditTimeFrame = null;
        private Frame _untransformedEditTimeFrame
        {
            get
            {
                if (_backingUntransformedEditTimeFrame == null)
                {
                    _backingUntransformedEditTimeFrame = new Frame();
                }
                return _backingUntransformedEditTimeFrame;
            }
        }
        private Frame _backingEditTimeFrame = null;
        private Frame _editTimeFrame
        {
            get
            {
                if (_backingEditTimeFrame == null)
                {
                    _backingEditTimeFrame = new Frame();
                }
                return _backingEditTimeFrame;
            }
        }

        private Dictionary<TestHandFactory.TestHandPose, Hand> _cachedLeftHands
          = new Dictionary<TestHandFactory.TestHandPose, Hand>();
        private Hand _editTimeLeftHand
        {
            get
            {
                Hand cachedHand;
                if (_cachedLeftHands.TryGetValue(editTimePose, out cachedHand))
                {
                    return cachedHand;
                }
                else
                {
                    cachedHand = TestHandFactory.MakeTestHand(isLeft: true, pose: editTimePose);
                    _cachedLeftHands[editTimePose] = cachedHand;
                    return cachedHand;
                }
            }
        }

        private Dictionary<TestHandFactory.TestHandPose, Hand> _cachedRightHands
          = new Dictionary<TestHandFactory.TestHandPose, Hand>();
        private Hand _editTimeRightHand
        {
            get
            {
                Hand cachedHand;
                if (_cachedRightHands.TryGetValue(editTimePose, out cachedHand))
                {
                    return cachedHand;
                }
                else
                {
                    cachedHand = TestHandFactory.MakeTestHand(isLeft: false, pose: editTimePose);
                    _cachedRightHands[editTimePose] = cachedHand;
                    return cachedHand;
                }
            }
        }

#endif

        #endregion

        #region LeapProvider Implementation

        public override Frame CurrentFrame
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    _editTimeFrame.Hands.Clear();
                    _untransformedEditTimeFrame.Hands.Clear();
                    _untransformedEditTimeFrame.Hands.Add(_editTimeLeftHand);
                    _untransformedEditTimeFrame.Hands.Add(_editTimeRightHand);
                    transformFrame(_untransformedEditTimeFrame, _editTimeFrame);
                    return _editTimeFrame;
                }
#endif
                if (_frameOptimization == FrameOptimizationMode.ReusePhysicsForUpdate)
                {
                    return _transformedFixedFrame;
                }
                else
                {
                    return _transformedUpdateFrame;
                }
            }
        }

        public override Frame CurrentFixedFrame
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    _editTimeFrame.Hands.Clear();
                    _untransformedEditTimeFrame.Hands.Clear();
                    _untransformedEditTimeFrame.Hands.Add(_editTimeLeftHand);
                    _untransformedEditTimeFrame.Hands.Add(_editTimeRightHand);
                    transformFrame(_untransformedEditTimeFrame, _editTimeFrame);
                    return _editTimeFrame;
                }
#endif
                if (_frameOptimization == FrameOptimizationMode.ReuseUpdateForPhysics)
                {
                    return _transformedUpdateFrame;
                }
                else
                {
                    return _transformedFixedFrame;
                }
            }
        }

        #endregion

        #region Unity Events

        
        protected float CalcularAnguloHorizontal(Hand hand)
        {
            /* Regla 1: Desviación Horizontal (palma - muñeca - codo)
             * Arm.NextJoint - Arm.PrevJoint - hand.PalmPosition
             * 
             * DEF: Utiliza los valores centrales de la mano, la muñeca y la palma
             * Puede fallar porque los valores del codo son estimados cuando el sensor no ve
             * el codo. Además no reconoce los casos en los que el codo está hacia abajo, solo arriba
             */


            Arm arm = hand.Arm;
            int ajustePosicionFuncional;

            //Calculo del angulo horizontal
            //Obtener los 3 puntos de análisis sin su posición en Y (Proyección en el plano)
            Vector3 palmaSinY = hand.PalmPosition.ToVector3();
            Vector3 muñecaSinY = arm.NextJoint.ToVector3();
            Vector3 codoSinY = arm.PrevJoint.ToVector3();

            palmaSinY.Set(palmaSinY.x, 0, palmaSinY.z);
            muñecaSinY.Set(muñecaSinY.x, 0, muñecaSinY.z);
            codoSinY.Set(codoSinY.x, 0, codoSinY.z);

            //Restar los vectores
            Vector3 palma_muñeca_SinY = palmaSinY - muñecaSinY;
            Vector3 muñeca_codo_SinY = muñecaSinY - codoSinY;

            //Hallar el producto punto
            float productoPuntoSinY = Vector3.Dot(palma_muñeca_SinY, muñeca_codo_SinY);

            //Hallar magnitud de los vectores
            float mag_palma_muñeca_SinY = Vector3.Magnitude(palma_muñeca_SinY);
            float mag_muñeca_codo_SinY = Vector3.Magnitude(muñeca_codo_SinY);

            //Calculo de la direccion de la mano (es necesario porque el angulo viene sin signo)
            // -1 para izquierda / 1 para derecha 
            float direccionMuñeca = Mathf.Sign(palmaSinY.x - muñecaSinY.x + 6) * -1;
            //16

            //Ajustar la desviacion cubital
            //Por definición la desviación cubital para una posición funcional son diez grados
            //el ajuste causa que el cero se encuentre al hacer una ligera desviación.
            if (hand.IsLeft)
            {
                ajustePosicionFuncional = 10; 
            }
            else
            {
                ajustePosicionFuncional = -10;
            }
            
            //Obtener coseno del angulo
            float cosAnguloSinY = productoPuntoSinY / (mag_palma_muñeca_SinY * mag_muñeca_codo_SinY);
            //Obtener el inverso del coseno, convertir a grados, agregar 
            float anguloHorizontal = Mathf.Round((Mathf.Abs(Mathf.Acos(cosAnguloSinY) * Mathf.Rad2Deg)  * direccionMuñeca) + ajustePosicionFuncional);


            return anguloHorizontal;
        }

        protected float CalcularAnguloVertical(Hand hand)
        {
            Arm arm = hand.Arm;

            //Calculo del angulo vertical
            //Obtener los 3 puntos de análisis
            Vector3 palmaSinX = hand.PalmPosition.ToVector3();
            palmaSinX.Set(0, palmaSinX.y, palmaSinX.z);
            Vector3 muñecaSinX = arm.NextJoint.ToVector3();
            muñecaSinX.Set(0, muñecaSinX.y, muñecaSinX.z);
            Vector3 codoSinX = arm.PrevJoint.ToVector3();
            codoSinX.Set(0, codoSinX.y, codoSinX.z);

            //Restar los vectores
            Vector3 palma_muñeca_SinX = palmaSinX - muñecaSinX;
            Vector3 muñeca_codo_SinX = muñecaSinX - codoSinX;

            //Hallar el producto punto
            float productoPuntoSinX = Vector3.Dot(palma_muñeca_SinX, muñeca_codo_SinX);

            //Hallar magnitud de los vectores
            float mag_palma_muñeca_SinX = Vector3.Magnitude(palma_muñeca_SinX);
            float mag_muñeca_codo_SinX = Vector3.Magnitude(muñeca_codo_SinX);

            //Obtener el ángulo
            float cosAnguloSinX = productoPuntoSinX / (mag_palma_muñeca_SinX * mag_muñeca_codo_SinX);
            float anguloVertical = Mathf.Acos(cosAnguloSinX) * Mathf.Rad2Deg - 5;

            return anguloVertical;
        }

        protected float CalcularPuntajeAgarre(Hand hand)
        {
            if (!hand.GetIndex().IsExtended || !hand.GetMiddle().IsExtended)// || !hand.GetRing().IsExtended)
            {
                return 0;
            }
            else
            {
                return 100;
            }
            //float puntajeGarra = Mathf.Round(hand.GetFistStrength() * 100);// - Mathf.Round(hand.PinchStrength * 30);
            
        }

        protected Incidente EvaluarDesviacionHorizontal(float anguloHorizontal, GameObject desvHorizontal, Incidente incHorizontal)
        {
            /* Regla 1: Desviación Horizontal
             * 
             * Parametros: 
             * float anguloHorizontal -> Angulo al cual se encuentra la palma desviada con respecto a la muñeca y el codo
             * bool isLeft -> Verdadero si la mano a evaluar es la izquierda o la derecha
             * GameObject desvHorizontal -> Objeto de la interfaz al cual se le actualizará el gráfico
             * 
             * DEF: Basado en los umbrales establecidos, la funcion retorna si la posición es calificada como riesgo
             * de salud ocupacional y adicionalmente deja en persistencia el evento registrado
             */

            anguloHorizontal = Mathf.Abs(anguloHorizontal);

            if (anguloHorizontal < 13)
            {
                desvHorizontal.GetComponent<UnityEngine.UI.Image>().sprite = desvHorizontalOK;
                if (incHorizontal.EstaActivo)//Si estaba fallando 
                {
                    incHorizontal.FinalizarIncidente();
                    dbstorage.InsertarIncidente(incHorizontal);
                }
            }
            else
            {
                desvHorizontal.GetComponent<UnityEngine.UI.Image>().sprite = desvHorizontalMala;
                incHorizontal.AvgDegreeArray.Add((int)anguloHorizontal);
                if (!incHorizontal.EstaActivo)//Si no estaba fallando 
                {
                    incHorizontal.IniciarIncidente(2, 1); //RuleID = 2, Score = 1
                }
            }

            return incHorizontal;


        }

        protected Incidente EvaluarDesviacionVertical(float anguloVertical, GameObject desvVertical, Incidente incVertical)
        {

            if (anguloVertical <= 17)
            {
                if (anguloVertical >= -2) //Si cae en rango seguro (Grado 0)
                {
                    desvVertical.GetComponent<UnityEngine.UI.Image>().sprite = manoFlexionOK;
                    if (incVertical.EstaActivo)//Si estaba fallando 
                    {
                        incVertical.FinalizarIncidente();
                        dbstorage.InsertarIncidente(incVertical);
                        return incVertical;
                    }
                }
                else //Si cae en rango medio (Grado 1)
                {
                    desvVertical.GetComponent<UnityEngine.UI.Image>().sprite = manoFlexionBaja;
                    incVertical.AvgDegreeArray.Add((int)anguloVertical);
                    if (!incVertical.EstaActivo)//Si no estaba fallando para Grado 1
                    {
                        incVertical.IniciarIncidente(1,1);
                        return incVertical;
                    }

                }
            }
            else
            {
                desvVertical.GetComponent<UnityEngine.UI.Image>().sprite = manoFlexionAlta;
                incVertical.AvgDegreeArray.Add((int)anguloVertical);
                if (!incVertical.EstaActivo)//Si no estaba fallando para Grado 2
                {
                    incVertical.IniciarIncidente(1, 2);
                    return incVertical;
                }
            }
            return incVertical;
        }

        protected Incidente EvaluarPuntajeAgarre(float puntajeGarra, GameObject agarre, Incidente incAgarre)
        {

            if (puntajeGarra <= 50)
            {
                agarre.GetComponent<UnityEngine.UI.Image>().sprite = clawGrip;
                if (incAgarre.EstaActivo)//Si estaba fallando 
                {
                    incAgarre.FinalizarIncidente();
                    dbstorage.InsertarIncidente(incAgarre);
                    return incAgarre;
                }
            }
            else
            {
                agarre.GetComponent<UnityEngine.UI.Image>().sprite = palmGrip;
                if (!incAgarre.EstaActivo)//Si no estaba fallando 
                {
                    incAgarre.IniciarIncidente(3,1);
                    return incAgarre;
                }
            }
            return incAgarre;
        }

        protected virtual void Reset()
        {
            editTimePose = TestHandFactory.TestHandPose.DesktopModeA;
        }

        protected virtual void Awake()
        {
            _fixedOffset.delay = 0.4f;
            _smoothedTrackingLatency.SetBlend(0.99f, 0.0111f);
        }

        protected virtual void Start()
        {
            createController();
            _transformedUpdateFrame = new Frame();
            _transformedFixedFrame = new Frame();
            _untransformedUpdateFrame = new Frame();
            _untransformedFixedFrame = new Frame();
            
            
            incHorizontalManoIzq.Hand = "Left";
            incHorizontalManoDer.Hand = "Right";
            incVerticalManoIzq.Hand = "Left";
            incVerticalManoDer.Hand = "Right";
            //Imprimir ruta de la BD
            //Debug.Log(Application.persistentDataPath);
            dbstorage = new DataStorage();
      

        }

        protected virtual void Update()
        {
            if (_workerThreadProfiling)
            {
                LeapProfiling.Update();
            }

            if (!checkConnectionIntegrity()) { return; }

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isCompiling)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.LogWarning("Unity hot reloading not currently supported. Stopping Editor Playback.");
                return;
            }
#endif

            _fixedOffset.Update(Time.time - Time.fixedTime, Time.deltaTime);

            if (_frameOptimization == FrameOptimizationMode.ReusePhysicsForUpdate)
            {
                DispatchUpdateFrameEvent(_transformedFixedFrame);
                return;
            }

            if (_useInterpolation)
            {
#if !UNITY_ANDROID || UNITY_EDITOR
                _smoothedTrackingLatency.value = Mathf.Min(_smoothedTrackingLatency.value, 30000f);
                _smoothedTrackingLatency.Update((float)(_leapController.Now() - _leapController.FrameTimestamp()), Time.deltaTime);
#endif
                long timestamp = CalculateInterpolationTime() + (ExtrapolationAmount * 1000);
                _unityToLeapOffset = timestamp - (long)(Time.time * S_TO_NS);

                _leapController.GetInterpolatedFrameFromTime(_untransformedUpdateFrame, timestamp, CalculateInterpolationTime() - (BounceAmount * 1000));
            }
            else
            {
                _leapController.Frame(_untransformedUpdateFrame);
            }

            if (_untransformedUpdateFrame != null)
            {
                // transformFrame(_untransformedUpdateFrame, _transformedUpdateFrame);

                //DispatchUpdateFrameEvent(_transformedUpdateFrame);

                //Leyendo el frame actual
                Frame myframe = new Frame();
                
                myframe.Id = _untransformedUpdateFrame.Id;
                myframe.Timestamp = _untransformedUpdateFrame.Timestamp;
                myframe.CurrentFramesPerSecond = _untransformedUpdateFrame.CurrentFramesPerSecond;

                //Crear Lista de Manos
                List<Hand> myhandlist = new List<Hand>();
                
                //Si la mano no es visible en el dispositivo, finalizar todos los incidentes abiertos
                if (_untransformedUpdateFrame.Hands.Count() == 0)
                {
                    if (incHorizontalManoIzq.EstaActivo)
                    {
                        incHorizontalManoIzq.FinalizarIncidente();
                        incVerticalManoIzq.FinalizarIncidente();
                        dbstorage.InsertarIncidente(incHorizontalManoIzq);
                        dbstorage.InsertarIncidente(incVerticalManoIzq);
                        desvVerticalManoIzq.GetComponent<UnityEngine.UI.Image>().sprite = manoFlexionOK;
                        desvHorizontalManoIzq.GetComponent<UnityEngine.UI.Image>().sprite = desvHorizontalOK; 
                    }
                    if (incHorizontalManoDer.EstaActivo)
                    {
                        incHorizontalManoDer.FinalizarIncidente();
                        incVerticalManoDer.FinalizarIncidente();
                        dbstorage.InsertarIncidente(incHorizontalManoDer);
                        dbstorage.InsertarIncidente(incVerticalManoDer);
                        desvHorizontalManoDer.GetComponent<UnityEngine.UI.Image>().sprite = desvHorizontalOK;
                        desvVerticalManoDer.GetComponent<UnityEngine.UI.Image>().sprite = manoFlexionOK;
                    }
                }
                else
                {//Si solo es visible una mano
                    if (_untransformedUpdateFrame.Hands.Count() == 1)
                    {
                        foreach (Hand hand in _untransformedUpdateFrame.Hands)
                        {//identificar si es mano izquiera o derecha
                            if (!hand.IsLeft)//Si la mano es la derecha cerrar los de la izq (pq es la que no se ve)
                            {// cerrar todos los incidentes asociados a la mano correspondiente
                                if (incHorizontalManoIzq.EstaActivo)
                                {
                                    incHorizontalManoIzq.FinalizarIncidente();
                                    incVerticalManoIzq.FinalizarIncidente();
                                    dbstorage.InsertarIncidente(incHorizontalManoIzq);
                                    dbstorage.InsertarIncidente(incVerticalManoIzq);
                                }
                            }
                            else//Si la mano es la izq cerrar los de la der (pq es la que no se ve)
                            {
                                if (incHorizontalManoDer.EstaActivo)
                                {
                                    incHorizontalManoDer.FinalizarIncidente();
                                    incVerticalManoDer.FinalizarIncidente();
                                    dbstorage.InsertarIncidente(incHorizontalManoDer);
                                    dbstorage.InsertarIncidente(incVerticalManoDer);
                                }
                            }
                        }

                    }
                }

                foreach (Hand hand in _untransformedUpdateFrame.Hands)
                {

                    if (hand.IsLeft)
                    {
                        iconoDesviacionHorizontal = desvHorizontalManoIzq;
                        iconoDesviacionVertical = desvVerticalManoIzq;
                        incHorizontal = incHorizontalManoIzq;
                        incVertical = incVerticalManoIzq;
                        if (modoZurdo)
                        {
                            puntuacionAgarre = CalcularPuntajeAgarre(hand);
                        }
                    }
                    else
                    {
                        iconoDesviacionHorizontal = desvHorizontalManoDer;
                        iconoDesviacionVertical = desvVerticalManoDer;
                        incHorizontal = incHorizontalManoDer;
                        incVertical = incVerticalManoDer;
                        if (!modoZurdo)
                        {
                            puntuacionAgarre = CalcularPuntajeAgarre(hand);
                        }
                    }
                    

                    //Añadir mano a la lista de manos
                    myhandlist.Add(hand);

                    //Calcular el angulo horizontal y vertical de la mano
                    anguloHorizontal = CalcularAnguloHorizontal(hand);
                    anguloVertical = CalcularAnguloVertical(hand);


                    
                    //Evaluar reglas
                    incHorizontal = EvaluarDesviacionHorizontal(anguloHorizontal, iconoDesviacionHorizontal, incHorizontal);
                    incVertical = EvaluarDesviacionVertical(anguloVertical, iconoDesviacionVertical, incVertical);
                    incAgarre = EvaluarPuntajeAgarre(puntuacionAgarre, Grip, incAgarre);

                    //Actualizar Incidentes

                    if (hand.IsLeft)
                    {
                        IzqVert.text = Mathf.Round(anguloVertical).ToString();
                        IzqHoriz.text = Mathf.Round(anguloHorizontal).ToString();
                        incHorizontalManoIzq = incHorizontal;
                        incVerticalManoIzq = incVertical;

                    }
                    else
                    {
                        incHorizontalManoDer = incHorizontal;
                        incVerticalManoDer = incVertical;
                        DerVert.text = Mathf.Round(anguloVertical).ToString();
                        DerHoriz.text = Mathf.Round(anguloHorizontal).ToString();
                        GripScore.text = "I: "+hand.GetIndex().IsExtended.ToString()+
                                         " - C: "+hand.GetMiddle().IsExtended.ToString()+
                                         " - A: "+hand.GetRing().IsExtended.ToString()+
                                         " - M: "+hand.GetPinky().IsExtended.ToString();
                    }


                }
                
                myframe.Hands = myhandlist;
                transformFrame(myframe, _transformedUpdateFrame);
                DispatchUpdateFrameEvent(_transformedUpdateFrame);


            }
        }

        protected virtual void FixedUpdate()
        {
            if (_frameOptimization == FrameOptimizationMode.ReuseUpdateForPhysics)
            {
                DispatchFixedFrameEvent(_transformedUpdateFrame);
                return;
            }

            if (_useInterpolation)
            {

                long timestamp;
                switch (_frameOptimization)
                {
                    case FrameOptimizationMode.None:
                        // By default we use Time.fixedTime to ensure that our hands are on the same
                        // timeline as Update.  We add an extrapolation value to help compensate
                        // for latency.
                        float extrapolatedTime = Time.fixedTime + CalculatePhysicsExtrapolation();
                        timestamp = (long)(extrapolatedTime * S_TO_NS) + _unityToLeapOffset;
                        break;
                    case FrameOptimizationMode.ReusePhysicsForUpdate:
                        // If we are re-using physics frames for update, we don't even want to care
                        // about Time.fixedTime, just grab the most recent interpolated timestamp
                        // like we are in Update.
                        timestamp = CalculateInterpolationTime() + (ExtrapolationAmount * 1000);
                        break;
                    default:
                        throw new System.InvalidOperationException(
                          "Unexpected frame optimization mode: " + _frameOptimization);
                }
                _leapController.GetInterpolatedFrame(_untransformedFixedFrame, timestamp);

            }
            else
            {
                _leapController.Frame(_untransformedFixedFrame);
            }

            if (_untransformedFixedFrame != null)
            {
                transformFrame(_untransformedFixedFrame, _transformedFixedFrame);

                DispatchFixedFrameEvent(_transformedFixedFrame);
                //Frame myframe = new Frame();
                //myframe.Id = _untransformedUpdateFrame.Id;
                //myframe.Timestamp = _untransformedUpdateFrame.Timestamp;
                //myframe.CurrentFramesPerSecond = _untransformedUpdateFrame.CurrentFramesPerSecond;

                ////Crear Lista de Manos
                //List<Hand> myhandlist = new List<Hand>();

                ////Copiando los valores
                //foreach (Hand hand in _untransformedUpdateFrame.Hands)
                //{
                //    if (hand.IsLeft)
                //    {
                //        hand.Transform(new LeapTransform(new Vector(0, 10, 0), LeapQuaternion.Identity));
                //    }

                //    myhandlist.Add(hand);

                //}

                //myframe.Hands = myhandlist;
                //transformFrame(myframe, _transformedUpdateFrame);
                //DispatchUpdateFrameEvent(_transformedUpdateFrame);

            }
        }

        protected virtual void OnDestroy()
        {
            destroyController();
            _isDestroyed = true;
            
        }

        protected virtual void OnApplicationPause(bool isPaused)
        {
            if (_leapController != null)
            {
                if (isPaused)
                {
                    _leapController.StopConnection();
                }
                else
                {
                    _leapController.StartConnection();
                }
            }
        }

        protected virtual void OnApplicationQuit()
        {
            destroyController();
            _isDestroyed = true;
            
        }

        public float CalculatePhysicsExtrapolation()
        {
            switch (_physicsExtrapolation)
            {
                case PhysicsExtrapolationMode.None:
                    return 0;
                case PhysicsExtrapolationMode.Auto:
                    return Time.fixedDeltaTime;
                case PhysicsExtrapolationMode.Manual:
                    return _physicsExtrapolationTime;
                default:
                    throw new System.InvalidOperationException(
                      "Unexpected physics extrapolation mode: " + _physicsExtrapolation);
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Returns the Leap Controller instance.
        /// </summary>
        public Controller GetLeapController()
        {
#if UNITY_EDITOR
            // Null check to deal with hot reloading.
            if (!_isDestroyed && _leapController == null)
            {
                createController();
            }
#endif
            return _leapController;
        }

        /// <summary>
        /// Returns true if the Leap Motion hardware is plugged in and this application is
        /// connected to the Leap Motion service.
        /// </summary>
        public bool IsConnected()
        {
            return GetLeapController().IsConnected;
        }

        /// <summary>
        /// Retransforms hand data from Leap space to the space of the Unity transform.
        /// This is only necessary if you're moving the LeapServiceProvider around in a
        /// custom script and trying to access Hand data from it directly afterward.
        /// </summary>
        public void RetransformFrames()
        {
            transformFrame(_untransformedUpdateFrame, _transformedUpdateFrame);
            transformFrame(_untransformedFixedFrame, _transformedFixedFrame);
        }

        /// <summary>
        /// Copies property settings from this LeapServiceProvider to the target
        /// LeapXRServiceProvider where applicable. Does not modify any XR-specific settings
        /// that only exist on the LeapXRServiceProvider.
        /// </summary>
        public void CopySettingsToLeapXRServiceProvider(
            LeapXRServiceProvider leapXRServiceProvider)
        {
            //leapXRServiceProvider._frameOptimization = _frameOptimization;
            //leapXRServiceProvider._physicsExtrapolation = _physicsExtrapolation;
            //leapXRServiceProvider._physicsExtrapolationTime = _physicsExtrapolationTime;
            //leapXRServiceProvider._workerThreadProfiling = _workerThreadProfiling;
        }

        #endregion

        #region Internal Methods

        protected virtual long CalculateInterpolationTime(bool endOfFrame = false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
      return _leapController.Now() - 16000;
#else
            if (_leapController != null)
            {
                return _leapController.Now() - (long)_smoothedTrackingLatency.value;
            }
            else
            {
                return 0;
            }
#endif
        }

        /// <summary>
        /// Initializes Leap Motion policy flags.
        /// </summary>
        protected virtual void initializeFlags()
        {
            if (_leapController == null)
            {
                return;
            }

            _leapController.ClearPolicy(Controller.PolicyFlag.POLICY_DEFAULT);
        }

        /// <summary>
        /// Creates an instance of a Controller, initializing its policy flags and
        /// subscribing to its connection event.
        /// </summary>
        protected void createController()
        {
            if (_leapController != null)
            {
                return;
            }

            _leapController = new Controller();
            _leapController.Device += (s, e) =>
            {
                if (_onDeviceSafe != null)
                {
                    _onDeviceSafe(e.Device);
                }
            };

            if (_leapController.IsConnected)
            {
                initializeFlags();
            }
            else
            {
                _leapController.Device += onHandControllerConnect;
            }

            if (_workerThreadProfiling)
            {
                //A controller will report profiling statistics for the duration of it's lifetime
                //so these events will never be unsubscribed from.
                _leapController.EndProfilingBlock += LeapProfiling.EndProfilingBlock;
                _leapController.BeginProfilingBlock += LeapProfiling.BeginProfilingBlock;

                _leapController.EndProfilingForThread += LeapProfiling.EndProfilingForThread;
                _leapController.BeginProfilingForThread += LeapProfiling.BeginProfilingForThread;
            }

            _leapController.SetPolicy(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
        }

        /// <summary>
        /// Stops the connection for the existing instance of a Controller, clearing old
        /// policy flags and resetting the Controller to null.
        /// </summary>
        protected void destroyController()
        {
            if (_leapController != null)
            {
                if (_leapController.IsConnected)
                {
                    _leapController.ClearPolicy(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                }
                _leapController.StopConnection();
                _leapController = null;
            }
        }

        private int _framesSinceServiceConnectionChecked = 0;
        private int _numberOfReconnectionAttempts = 0;
        /// <summary>
        /// Checks whether this provider is connected to a service;
        /// If it is not, attempt to reconnect at regular intervals
        /// for MAX_RECONNECTION_ATTEMPTS
        /// </summary>
        protected bool checkConnectionIntegrity()
        {
            if (_leapController.IsServiceConnected)
            {
                _framesSinceServiceConnectionChecked = 0;
                _numberOfReconnectionAttempts = 0;
                return true;
            }
            else if (_numberOfReconnectionAttempts < MAX_RECONNECTION_ATTEMPTS)
            {
                _framesSinceServiceConnectionChecked++;

                if (_framesSinceServiceConnectionChecked > RECONNECTION_INTERVAL)
                {
                    _framesSinceServiceConnectionChecked = 0;
                    _numberOfReconnectionAttempts++;

                    Debug.LogWarning("Leap Service not connected; attempting to reconnect for try " +
                                     _numberOfReconnectionAttempts + "/" + MAX_RECONNECTION_ATTEMPTS +
                                     "...", this);
                    using (new ProfilerSample("Reconnection Attempt"))
                    {
                        destroyController();
                        createController();
                    }
                }
            }
            return false;
        }

        protected void onHandControllerConnect(object sender, LeapEventArgs args)
        {
            initializeFlags();

            if (_leapController != null)
            {
                _leapController.Device -= onHandControllerConnect;
            }
        }

        protected virtual void transformFrame(Frame source, Frame dest)
        {
            dest.CopyFrom(source).Transform(transform.GetLeapMatrix());
        }

        #endregion

    }

}
