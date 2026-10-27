using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;


// =========================================================
// LIBRERIAS EXCLUSIVAS DE WINDOWS
// =========================================================

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System.Runtime.InteropServices;
using System.Text;

#endif


public class LanzadorJuegoPCVR : MonoBehaviour
{
    // =====================================================
    // REGRESO AL LOBBY
    // =====================================================
    //
    // MISMO BOTON Y.
    //
    // WINDOWS / QUEST LINK:
    //
    //      abre el EXE del Lobby
    //
    // QUEST STANDALONE:
    //
    //      abre el APK del Lobby
    //
    // =====================================================

    [Header("Regreso al Lobby")]

    [Tooltip(
        "Ruta relativa desde el EXE del juego actual " +
        "hasta el EXE del Lobby."
    )]
    public string rutaRelativaLobby =
        @"..\..\LOBBYEXES\LobbyCasitasDelTerror.exe";


    [Tooltip(
        "Package Name del APK del Lobby instalado en Meta Quest."
    )]
    public string paqueteLobbyQuest =
        "com.casitasdelterror.lobby";


    // =====================================================
    // TIEMPOS
    // =====================================================

    [Header("Tiempos")]

    public float esperaLiberarXR = 0.8f;

    public float esperaConfirmarProceso = 1.5f;


    // =====================================================
    // VARIABLES INTERNAS
    // =====================================================

    private InputDevice controlIzquierdo;

    private bool yAnterior = false;

    private bool cambiandoAplicacion = false;


    // =====================================================
    // WINDOWS API
    // =====================================================

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

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


    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();


    [DllImport(
        "kernel32.dll",
        SetLastError = true
    )]
    private static extern System.IntPtr OpenProcess(
        uint dwDesiredAccess,
        bool bInheritHandle,
        uint dwProcessId
    );


    [DllImport(
        "kernel32.dll",
        SetLastError = true
    )]
    private static extern bool TerminateProcess(
        System.IntPtr hProcess,
        uint uExitCode
    );


    private const uint WAIT_TIMEOUT =
        0x00000102;


    private const uint PROCESS_TERMINATE =
        0x0001;

#endif


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        Application.runInBackground = true;


        // =================================================
        // MOSTRAR AMBIENTE DETECTADO
        // =================================================

#if UNITY_ANDROID && !UNITY_EDITOR

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "AMBIENTE DETECTADO: META QUEST APK"
        );

        Debug.Log(
            "LOBBY QUEST: " +
            paqueteLobbyQuest
        );

        Debug.Log(
            "========================================"
        );

#elif UNITY_STANDALONE_WIN

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "AMBIENTE DETECTADO: PC / QUEST LINK"
        );

        Debug.Log(
            "LOBBY PC: " +
            rutaRelativaLobby
        );

        Debug.Log(
            "========================================"
        );

#elif UNITY_EDITOR_WIN

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "AMBIENTE DETECTADO: UNITY EDITOR WINDOWS"
        );

        Debug.Log(
            "========================================"
        );

#endif


        BuscarControlIzquierdo();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (cambiandoAplicacion)
            return;


        if (!controlIzquierdo.isValid)
        {
            BuscarControlIzquierdo();
        }


        LeerBotonY();
    }


    // =====================================================
    // BUSCAR CONTROL IZQUIERDO
    // =====================================================

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
    // BOTON Y
    // =====================================================

    void LeerBotonY()
    {
        if (!controlIzquierdo.isValid)
            return;


        bool yPresionado = false;


        bool recibido =
            controlIzquierdo.TryGetFeatureValue(
                CommonUsages.secondaryButton,
                out yPresionado
            );


        if (!recibido)
            return;


        // =================================================
        // DETECTAR UNA SOLA PULSACION
        // =================================================

        if (yPresionado && !yAnterior)
        {
            Debug.Log(
                "========================================"
            );

            Debug.Log(
                "BOTON Y PRESIONADO -> REGRESANDO AL LOBBY"
            );

            Debug.Log(
                "========================================"
            );


            RegresarAlLobby();
        }


        yAnterior =
            yPresionado;
    }


    // =====================================================
    // REGRESAR AL LOBBY
    // =====================================================
    //
    // ESTA FUNCION DETECTA AUTOMATICAMENTE
    // EL AMBIENTE.
    //
    // =====================================================

    void RegresarAlLobby()
    {
        if (cambiandoAplicacion)
            return;


        // =================================================
        // META QUEST STANDALONE
        // =================================================

#if UNITY_ANDROID && !UNITY_EDITOR

        Debug.Log(
            "META QUEST DETECTADO"
        );


        Debug.Log(
            "ABRIENDO APK DEL LOBBY:\n" +
            paqueteLobbyQuest
        );


        if (string.IsNullOrWhiteSpace(
            paqueteLobbyQuest
        ))
        {
            Debug.LogError(
                "EL PACKAGE DEL LOBBY ESTA VACIO"
            );

            return;
        }


        StartCoroutine(
            CambiarAplicacionQuest(
                paqueteLobbyQuest.Trim()
            )
        );


        // =================================================
        // WINDOWS / QUEST LINK / AIR LINK
        // =================================================

#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

        Debug.Log(
            "WINDOWS / PCVR DETECTADO"
        );


        Debug.Log(
            "ABRIENDO EXE DEL LOBBY:\n" +
            rutaRelativaLobby
        );


        LanzarRuta(
            rutaRelativaLobby
        );


        // =================================================
        // OTRA PLATAFORMA
        // =================================================

#else

        Debug.LogError(
            "PLATAFORMA NO SOPORTADA: " +
            Application.platform
        );

#endif
    }


    // =====================================================
    // WINDOWS
    // LANZAR RUTA RELATIVA
    // =====================================================

    public void LanzarRuta(
        string rutaRelativa
    )
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

        if (cambiandoAplicacion)
        {
            Debug.LogWarning(
                "YA HAY UN CAMBIO DE APLICACION EN CURSO"
            );

            return;
        }


        if (string.IsNullOrWhiteSpace(
            rutaRelativa
        ))
        {
            Debug.LogError(
                "LA RUTA RELATIVA ESTA VACIA"
            );

            return;
        }


        // =================================================
        // CARPETA DEL EXE ACTUAL
        // =================================================

        string carpetaExeActual =
            Directory.GetParent(
                Application.dataPath
            ).FullName;


        // =================================================
        // RESOLVER RUTA RELATIVA
        // =================================================

        string rutaCompleta =
            Path.GetFullPath(
                Path.Combine(
                    carpetaExeActual,
                    rutaRelativa.Trim()
                )
            );


        Debug.Log(
            "CARPETA DEL EXE ACTUAL:\n" +
            carpetaExeActual
        );


        Debug.Log(
            "RUTA RELATIVA RECIBIDA:\n" +
            rutaRelativa
        );


        Debug.Log(
            "RUTA FINAL:\n" +
            rutaCompleta
        );


        StartCoroutine(
            CambiarAplicacionWindows(
                rutaCompleta
            )
        );

#endif
    }


    // =====================================================
    // CAMBIAR APLICACION WINDOWS
    // =====================================================

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

    IEnumerator CambiarAplicacionWindows(
        string rutaDestino
    )
    {
        cambiandoAplicacion = true;


        // =================================================
        // 1. VERIFICAR DESTINO
        // =================================================

        if (!File.Exists(
            rutaDestino
        ))
        {
            Debug.LogError(
                "NO EXISTE EL EXE:\n" +
                rutaDestino
            );


            cambiandoAplicacion =
                false;


            yield break;
        }


        Debug.Log(
            "DESTINO ENCONTRADO:\n" +
            rutaDestino
        );


        // =================================================
        // 2. LIBERAR OPENXR
        // =================================================

        XRManagerSettings manager =
            null;


        if (XRGeneralSettings.Instance != null)
        {
            manager =
                XRGeneralSettings
                    .Instance
                    .Manager;
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


        yield return
            new WaitForSecondsRealtime(
                esperaLiberarXR
            );


        // =================================================
        // 3. PREPARAR EXE
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


        // =================================================
        // 4. ABRIR EXE DEL LOBBY
        // =================================================

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
                "NO SE PUDO ABRIR EL LOBBY. " +
                "ERROR WINDOWS: " +
                error
            );


            cambiandoAplicacion =
                false;


            yield return
                StartCoroutine(
                    RecuperarXR()
                );


            yield break;
        }


        // =================================================
        // CERRAR THREAD HANDLE
        // =================================================

        CloseHandle(
            proceso.hThread
        );


        Debug.Log(
            "LOBBY EXE ABIERTO. PID: " +
            proceso.dwProcessId
        );


        // =================================================
        // 5. ESPERAR
        // =================================================

        yield return
            new WaitForSecondsRealtime(
                esperaConfirmarProceso
            );


        // =================================================
        // 6. CONFIRMAR QUE EL LOBBY SIGUE VIVO
        // =================================================

        bool sigueVivo =
            WaitForSingleObject(
                proceso.hProcess,
                0
            ) == WAIT_TIMEOUT;


        if (!sigueVivo)
        {
            Debug.LogError(
                "EL LOBBY SE CERRO AL INICIAR. " +
                "NO CIERRO EL JUEGO ACTUAL."
            );


            CloseHandle(
                proceso.hProcess
            );


            cambiandoAplicacion =
                false;


            yield return
                StartCoroutine(
                    RecuperarXR()
                );


            yield break;
        }


        CloseHandle(
            proceso.hProcess
        );


        Debug.Log(
            "LOBBY ABIERTO CORRECTAMENTE."
        );


        Debug.Log(
            "CERRANDO JUEGO ACTUAL..."
        );


        // =================================================
        // 7. CERRAR JUEGO ACTUAL
        // =================================================

        CerrarAplicacionActualWindows();
    }


    // =====================================================
    // CERRAR APLICACION WINDOWS
    // =====================================================

    void CerrarAplicacionActualWindows()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying =
            false;

#else

        uint pid =
            GetCurrentProcessId();


        System.IntPtr handle =
            OpenProcess(
                PROCESS_TERMINATE,
                false,
                pid
            );


        if (handle != System.IntPtr.Zero)
        {
            TerminateProcess(
                handle,
                0
            );


            CloseHandle(
                handle
            );
        }
        else
        {
            Application.Quit();
        }

#endif
    }

#endif


    // =====================================================
    // META QUEST STANDALONE
    // =====================================================

#if UNITY_ANDROID && !UNITY_EDITOR

    IEnumerator CambiarAplicacionQuest(
        string paqueteQuest
    )
    {
        cambiandoAplicacion =
            true;


        // =================================================
        // 1. VALIDAR PACKAGE
        // =================================================

        if (string.IsNullOrWhiteSpace(
            paqueteQuest
        ))
        {
            Debug.LogError(
                "PACKAGE DEL LOBBY VACIO"
            );


            cambiandoAplicacion =
                false;


            yield break;
        }


        Debug.Log(
            "PACKAGE LOBBY:\n" +
            paqueteQuest
        );


        // =================================================
        // 2. LIBERAR OPENXR
        // =================================================

        XRManagerSettings manager =
            null;


        if (XRGeneralSettings.Instance != null)
        {
            manager =
                XRGeneralSettings
                    .Instance
                    .Manager;
        }


        if (manager != null &&
            manager.isInitializationComplete)
        {
            Debug.Log(
                "LIBERANDO XR QUEST..."
            );


            manager.StopSubsystems();


            manager.DeinitializeLoader();
        }


        yield return
            new WaitForSecondsRealtime(
                esperaLiberarXR
            );


        bool abierto =
            false;


        // =================================================
        // 3. ABRIR APK DEL LOBBY
        // =================================================

        try
        {
            using (
                AndroidJavaClass unityPlayer =
                    new AndroidJavaClass(
                        "com.unity3d.player.UnityPlayer"
                    )
            )
            {
                AndroidJavaObject actividadActual =
                    unityPlayer
                        .GetStatic<AndroidJavaObject>(
                            "currentActivity"
                        );


                if (actividadActual == null)
                {
                    Debug.LogError(
                        "NO SE ENCONTRO currentActivity"
                    );
                }
                else
                {
                    AndroidJavaObject packageManager =
                        actividadActual.Call<AndroidJavaObject>(
                            "getPackageManager"
                        );


                    // =====================================
                    // PRIMERO INTENTAMOS OBTENER EL
                    // LAUNCH INTENT DEL PACKAGE
                    // =====================================

                    AndroidJavaObject intent =
                        packageManager
                            .Call<AndroidJavaObject>(
                                "getLaunchIntentForPackage",
                                paqueteQuest
                            );


                    // =====================================
                    // SI NO LO ENCUENTRA,
                    // CREAMOS UN INTENT MANUAL
                    // =====================================

                    if (intent == null)
                    {
                        Debug.LogWarning(
                            "getLaunchIntentForPackage " +
                            "NO ENCONTRO EL LOBBY. " +
                            "INTENTANDO INTENT MANUAL."
                        );


                        intent =
                            new AndroidJavaObject(
                                "android.content.Intent",
                                "android.intent.action.MAIN"
                            );


                        intent.Call<AndroidJavaObject>(
                            "addCategory",
                            "android.intent.category.LAUNCHER"
                        );


                        intent.Call<AndroidJavaObject>(
                            "setPackage",
                            paqueteQuest
                        );
                    }


                    if (intent != null)
                    {
                        // =================================
                        // FLAG_ACTIVITY_NEW_TASK
                        // =================================

                        intent.Call<AndroidJavaObject>(
                            "addFlags",
                            0x10000000
                        );


                        Debug.Log(
                            "ABRIENDO LOBBY APK:\n" +
                            paqueteQuest
                        );


                        actividadActual.Call(
                            "startActivity",
                            intent
                        );


                        abierto =
                            true;


                        Debug.Log(
                            "LOBBY APK LANZADO"
                        );


                        // =================================
                        // CERRAR JUEGO ACTUAL
                        // =================================

                        actividadActual.Call(
                            "finish"
                        );


                        Debug.Log(
                            "JUEGO ACTUAL FINALIZADO"
                        );


                        intent.Dispose();
                    }


                    if (packageManager != null)
                    {
                        packageManager.Dispose();
                    }


                    actividadActual.Dispose();
                }
            }
        }
        catch (
            System.Exception error
        )
        {
            Debug.LogError(
                "ERROR AL ABRIR EL LOBBY APK:\n" +
                error
            );


            abierto =
                false;
        }


        // =================================================
        // 4. SI FALLA, RECUPERAR XR
        // =================================================

        if (!abierto)
        {
            Debug.LogError(
                "NO SE PUDO ABRIR EL LOBBY APK:\n" +
                paqueteQuest
            );


            cambiandoAplicacion =
                false;


            yield return
                StartCoroutine(
                    RecuperarXR()
                );
        }
    }

#endif


    // =====================================================
    // RECUPERAR XR SI FALLA EL CAMBIO
    // =====================================================

    IEnumerator RecuperarXR()
    {
        Debug.Log(
            "RECUPERANDO XR..."
        );


        if (XRGeneralSettings.Instance == null)
        {
            Debug.LogError(
                "XRGeneralSettings ES NULL"
            );

            yield break;
        }


        XRManagerSettings manager =
            XRGeneralSettings
                .Instance
                .Manager;


        if (manager == null)
        {
            Debug.LogError(
                "XR MANAGER ES NULL"
            );

            yield break;
        }


        yield return
            manager.InitializeLoader();


        if (manager.activeLoader == null)
        {
            Debug.LogError(
                "NO SE PUDO RECUPERAR XR"
            );

            yield break;
        }


        manager.StartSubsystems();


        Debug.Log(
            "XR RECUPERADO"
        );
    }
}