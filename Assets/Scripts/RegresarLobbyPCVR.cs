using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class LanzadorJuegoPCVR : MonoBehaviour
{
    public enum TipoDestino
    {
        Equipo1,
        Lobby
    }

    [Header("Destino")]
    public TipoDestino destino = TipoDestino.Equipo1;

    [Header("Regreso desde juego")]
    public bool regresarConYIzquierda = false;

    [Header("Tiempos")]
    public float esperaLiberarXR = 0.8f;
    public float esperaConfirmarProceso = 1.5f;


    // =====================================================
    // RUTAS
    // =====================================================

    private const string RUTA_EQUIPO1 =
        @"C:\Users\Wazab\OneDrive\Documents\Unity\Equipo1\EXEEQUIPO1\Equipo1Juegito.exe";

    private const string RUTA_LOBBY =
        @"C:\xampp\htdocs\LobbyVR\LobbyCasitasDelTerror\LOBBYEXES\LobbyCasitasDelTerror.exe";


    // =====================================================
    // WINDOWS
    // =====================================================

    [StructLayout(LayoutKind.Sequential)]
    private struct STARTUPINFO
    {
        public int cb;
        public System.IntPtr lpReserved;
        public System.IntPtr lpDesktop;
        public System.IntPtr lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public System.IntPtr lpReserved2;
        public System.IntPtr hStdInput;
        public System.IntPtr hStdOutput;
        public System.IntPtr hStdError;
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public System.IntPtr hProcess;
        public System.IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }


    [DllImport(
        "kernel32.dll",
        CharSet = CharSet.Unicode,
        SetLastError = true
    )]
    private static extern bool CreateProcessW(
        string lpApplicationName,
        StringBuilder lpCommandLine,
        System.IntPtr lpProcessAttributes,
        System.IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        System.IntPtr lpEnvironment,
        string lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation
    );


    [DllImport(
        "kernel32.dll",
        SetLastError = true
    )]
    private static extern uint WaitForSingleObject(
        System.IntPtr hHandle,
        uint dwMilliseconds
    );


    [DllImport(
        "kernel32.dll",
        SetLastError = true
    )]
    private static extern bool CloseHandle(
        System.IntPtr hObject
    );


    private const uint WAIT_TIMEOUT = 0x00000102;


    // =====================================================
    // META QUEST
    // =====================================================

    private InputDevice controlIzquierdo;
    private bool yAnterior = false;

    private bool cambiandoAplicacion = false;


    void Start()
    {
        Application.runInBackground = true;

        if (regresarConYIzquierda)
        {
            BuscarControlIzquierdo();
        }
    }


    void Update()
    {
        // En el Lobby esto estará apagado.
        if (!regresarConYIzquierda)
            return;


        if (!controlIzquierdo.isValid)
        {
            BuscarControlIzquierdo();
        }


        bool yPresionado = false;


        bool recibido =
            controlIzquierdo.TryGetFeatureValue(
                CommonUsages.secondaryButton,
                out yPresionado
            );


        if (!recibido)
            return;


        // Y del control izquierdo.
        if (yPresionado && !yAnterior)
        {
            Debug.Log(
                "Y IZQUIERDA -> REGRESANDO AL LOBBY"
            );

            LanzarJuego();
        }


        yAnterior = yPresionado;
    }


    void BuscarControlIzquierdo()
    {
        controlIzquierdo =
            InputDevices.GetDeviceAtXRNode(
                XRNode.LeftHand
            );


        if (controlIzquierdo.isValid)
        {
            Debug.Log(
                "CONTROL IZQUIERDO DETECTADO: " +
                controlIzquierdo.name
            );
        }
    }


    // =====================================================
    // ESTA ES LA FUNCIÓN QUE USA MANOHORROR
    // =====================================================

    public void LanzarJuego()
    {
        if (cambiandoAplicacion)
            return;


        StartCoroutine(
            CambiarAplicacion()
        );
    }


    IEnumerator CambiarAplicacion()
    {
        cambiandoAplicacion = true;


        string rutaDestino =
            ObtenerRuta();


        // =================================================
        // 1. VERIFICAR DESTINO
        // =================================================

        if (!File.Exists(rutaDestino))
        {
            Debug.LogError(
                "NO EXISTE EL EXE:\n" +
                rutaDestino
            );


            cambiandoAplicacion = false;

            yield break;
        }


        Debug.Log(
            "DESTINO ENCONTRADO:\n" +
            rutaDestino
        );


        // =================================================
        // 2. LIBERAR OPENXR
        // =================================================

        XRManagerSettings manager = null;


        if (XRGeneralSettings.Instance != null)
        {
            manager =
                XRGeneralSettings.Instance.Manager;
        }


        if (manager != null &&
            manager.isInitializationComplete)
        {
            Debug.Log(
                "LIBERANDO XR..."
            );


            manager.StopSubsystems();
            manager.DeinitializeLoader();
        }


        yield return new WaitForSecondsRealtime(
            esperaLiberarXR
        );


        // =================================================
        // 3. ABRIR EL OTRO EXE
        // =================================================

        STARTUPINFO startup =
            new STARTUPINFO();


        startup.cb =
            Marshal.SizeOf(
                typeof(STARTUPINFO)
            );


        StringBuilder lineaComandos =
            new StringBuilder(
                "\"" +
                rutaDestino +
                "\""
            );


        PROCESS_INFORMATION proceso;


        bool abierto =
            CreateProcessW(
                rutaDestino,
                lineaComandos,
                System.IntPtr.Zero,
                System.IntPtr.Zero,
                false,
                0,
                System.IntPtr.Zero,
                Path.GetDirectoryName(
                    rutaDestino
                ),
                ref startup,
                out proceso
            );


        if (!abierto)
        {
            int error =
                Marshal.GetLastWin32Error();


            Debug.LogError(
                "NO SE PUDO ABRIR EL DESTINO. " +
                "ERROR WINDOWS: " +
                error
            );


            cambiandoAplicacion = false;

            yield break;
        }


        // No necesitamos el thread handle.
        CloseHandle(
            proceso.hThread
        );


        Debug.Log(
            "NUEVO EXE ABIERTO. PID: " +
            proceso.dwProcessId
        );


        // =================================================
        // 4. ESPERAR Y CONFIRMAR QUE SIGUE VIVO
        // =================================================

        yield return new WaitForSecondsRealtime(
            esperaConfirmarProceso
        );


        bool sigueVivo =
            WaitForSingleObject(
                proceso.hProcess,
                0
            ) == WAIT_TIMEOUT;


        if (!sigueVivo)
        {
            Debug.LogError(
                "EL NUEVO EXE SE CERRO. " +
                "NO CIERRO LA APP ACTUAL."
            );


            CloseHandle(
                proceso.hProcess
            );


            cambiandoAplicacion = false;

            yield break;
        }


        CloseHandle(
            proceso.hProcess
        );


        // =================================================
        // 5. CERRAR ESTA APP
        // =================================================

        Debug.Log(
            "CAMBIO CORRECTO. CERRANDO APP ACTUAL."
        );


#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying =
            false;

#else

        Application.Quit();

#endif
    }


    string ObtenerRuta()
    {
        if (destino == TipoDestino.Equipo1)
        {
            return RUTA_EQUIPO1;
        }


        return RUTA_LOBBY;
    }
}