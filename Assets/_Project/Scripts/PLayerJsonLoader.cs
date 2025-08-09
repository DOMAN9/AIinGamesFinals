using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class PlayerJsonLoader : MonoBehaviour
{
    [Header("Server (read)")]
    [SerializeField] string dataUrl = "http://localhost:7047/api/data";
    [SerializeField] float refreshSeconds = 0f; // 0 = one-time GET

    [Header("Server (write)")]
    [SerializeField] string updateUrl = "http://localhost:7047/api/data";
    [SerializeField, Range(0.05f, 5f)] float sendInterval = 0.5f;
    [SerializeField] bool onlyWhenChanged = true;

    [Header("Your Components")]
    [SerializeField] PlayerUI playerUI;          // assign in Inspector
    [SerializeField] MonoBehaviour healthScript; // optional fallback
    [SerializeField] MonoBehaviour statsScript;  // optional fallback

    string lastPayloadHash = "";

    void Start()
    {
        if (refreshSeconds > 0f) StartCoroutine(GetLoop());
        else StartCoroutine(LoadOnce());

        StartCoroutine(SendLoop());
    }

    // -------- GET --------
    IEnumerator GetLoop()
    {
        while (true)
        {
            yield return LoadOnce();
            yield return new WaitForSeconds(refreshSeconds);
        }
    }

    IEnumerator LoadOnce()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(dataUrl))
        {
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[PlayerJsonLoader] GET failed: {req.responseCode} {req.error}\n{req.downloadHandler.text}");
                yield break;
            }

            ServerPlayerData data = JsonUtility.FromJson<ServerPlayerData>(req.downloadHandler.text);
            ApplyFromServer(data);
        }
    }

    void ApplyFromServer(ServerPlayerData d)
    {
        if (playerUI != null)
        {
            // Remove this line to stop overwriting the player name:
            // if (!string.IsNullOrEmpty(d.playerName)) playerUI.playerName = d.playerName;

            playerUI.playerScore = d.score;

            if (playerUI.playerTransform != null)
                playerUI.playerTransform.position = new Vector3(d.playerPosition.x, d.playerPosition.y, d.playerPosition.z);

            if (playerUI.playerHealthScript != null)
            {
                var t = playerUI.playerHealthScript.GetType();
                var m = t.GetMethod("SetHealth");
                if (m != null) m.Invoke(playerUI.playerHealthScript, new object[] { d.playerHealth });
                else
                {
                    var f = t.GetField("currentHealth") ?? t.GetField("health");
                    if (f != null) f.SetValue(playerUI.playerHealthScript, d.playerHealth);
                    else
                    {
                        var p = t.GetProperty("CurrentHealth") ?? t.GetProperty("Health");
                        if (p != null && p.CanWrite) p.SetValue(playerUI.playerHealthScript, d.playerHealth);
                    }
                }
            }
        }
        else
        {
            // Same for GameObject name — remove if you don't want it overwritten
            // if (!string.IsNullOrEmpty(d.playerName)) gameObject.name = d.playerName;

            transform.position = new Vector3(d.playerPosition.x, d.playerPosition.y, d.playerPosition.z);
        }

        Debug.Log("[PlayerJsonLoader] Applied server data (GET).");
    }


    // -------- POST --------
    IEnumerator SendLoop()
    {
        WaitForSeconds wait = new WaitForSeconds(sendInterval);
        while (true)
        {
            yield return SendOnce();
            yield return wait;
        }
    }

    IEnumerator SendOnce()
    {
        // Read from PlayerUI if available (single source of truth)
        string name = playerUI ? playerUI.playerName : gameObject.name;

        int hp = 0;
        if (playerUI && playerUI.playerHealthScript != null)
        {
            var t = playerUI.playerHealthScript.GetType();
            var p = t.GetProperty("CurrentHealth");
            if (p != null && p.PropertyType == typeof(int))
                hp = (int)p.GetValue(playerUI.playerHealthScript);
        }
        else
        {
            hp = ReadHealthFallback();
        }

        int sc = playerUI ? playerUI.playerScore : ReadScoreFallback();
        Vector3 pos = (playerUI && playerUI.playerTransform != null) ? playerUI.playerTransform.position : transform.position;

        ServerPlayerData payload = new ServerPlayerData
        {
            playerName = name,
            playerHealth = hp,
            score = sc,
            playerPosition = new Vec3 { x = pos.x, y = pos.y, z = pos.z }
        };

        string json = JsonUtility.ToJson(payload);

        if (onlyWhenChanged)
        {
            string h = SimpleHash(json);
            if (h == lastPayloadHash) yield break;
            lastPayloadHash = h;
        }

        using (UnityWebRequest req = new UnityWebRequest(updateUrl, UnityWebRequest.kHttpVerbPOST))
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.timeout = 10;

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[PlayerJsonLoader] POST failed: {req.responseCode} {req.error}\n{req.downloadHandler.text}");
            else
                Debug.Log("[PlayerJsonLoader] Sent update (POST) → " + json);
        }
    }

    // -------- helpers / fallbacks --------
    string SimpleHash(string s)
    {
        unchecked
        {
            int h = 23;
            for (int i = 0; i < s.Length; i++) h = h * 31 + s[i];
            return h.ToString("X");
        }
    }

    int ReadScoreFallback()
    {
        if (statsScript == null) return 0;
        var t = statsScript.GetType();

        var p = t.GetProperty("Score");
        if (p != null && p.PropertyType == typeof(int)) return (int)p.GetValue(statsScript);

        var f = t.GetField("score") ?? t.GetField("Score") ?? t.GetField("points");
        if (f != null && f.FieldType == typeof(int)) return (int)f.GetValue(statsScript);

        var m = t.GetMethod("GetScore");
        if (m != null) return (int)m.Invoke(statsScript, null);

        return 0;
    }

    int ReadHealthFallback()
    {
        if (healthScript == null) return 0;
        var t = healthScript.GetType();

        var p = t.GetProperty("CurrentHealth");
        if (p != null && p.PropertyType == typeof(int)) return (int)p.GetValue(healthScript);

        var f = t.GetField("currentHealth", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
              ?? t.GetField("health");
        if (f != null && f.FieldType == typeof(int)) return (int)f.GetValue(healthScript);

        var m = t.GetMethod("GetHealth");
        if (m != null) return (int)m.Invoke(healthScript, null);

        return 0;
    }
}

/* === JSON models (fields, not properties) === */
[System.Serializable]
public class ServerPlayerData
{
    public string playerName;
    public int playerHealth;
    public int score;
    public Vec3 playerPosition;
}

[System.Serializable]
public class Vec3 { public float x, y, z; }
