/**
 * Cloudflare Worker (OpenAI proxy)
 *
 * Required secrets:
 * - OPENAI_API_KEY
 *
 * Optional secrets:
 * - CLIENT_BEARER_TOKEN  (Unity -> Worker 인증 토큰, 비우면 인증 없이 허용)
 * - ALLOWED_ORIGIN       (CORS 허용 origin, 비우면 "*")
 *
 * Endpoints:
 * - POST /transcribe
 * - POST /normalize
 */

export default {
  async fetch(request, env) {
    if (request.method === "OPTIONS") {
      return withCors(new Response(null, { status: 204 }), env);
    }

    const url = new URL(request.url);

    if (url.pathname === "/transcribe") {
      return withCors(await handleTranscribe(request, env), env);
    }

    if (url.pathname === "/normalize") {
      return withCors(await handleNormalize(request, env), env);
    }

    return withCors(json({ error: "Not Found" }, 404), env);
  }
};

async function handleTranscribe(request, env) {
  if (request.method !== "POST") {
    return json({ error: "Method Not Allowed" }, 405);
  }

  if (!isClientAuthorized(request, env)) {
    return json({ error: "Unauthorized client" }, 401);
  }

  if (!env.OPENAI_API_KEY) {
    return json({ error: "OPENAI_API_KEY is not configured" }, 500);
  }

  try {
    const contentType = request.headers.get("Content-Type") || "";
    if (!contentType.toLowerCase().includes("multipart/form-data")) {
      return json({ error: "multipart/form-data request is required" }, 400);
    }

    const upstream = await fetch("https://api.openai.com/v1/audio/transcriptions", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${env.OPENAI_API_KEY}`,
        "Content-Type": contentType
      },
      body: request.body
    });

    const text = await upstream.text();
    return new Response(text, {
      status: upstream.status,
      headers: {
        "Content-Type": "application/json; charset=utf-8"
      }
    });
  } catch (e) {
    return json({ error: "Proxy transcribe failed", detail: String(e) }, 500);
  }
}

async function handleNormalize(request, env) {
  if (request.method !== "POST") {
    return json({ error: "Method Not Allowed" }, 405);
  }

  if (!isClientAuthorized(request, env)) {
    return json({ error: "Unauthorized client" }, 401);
  }

  if (!env.OPENAI_API_KEY) {
    return json({ error: "OPENAI_API_KEY is not configured" }, 500);
  }

  try {
    const bodyText = await request.text();
    if (!bodyText || !bodyText.trim()) {
      return json({ error: "body is required" }, 400);
    }

    const upstream = await fetch("https://api.openai.com/v1/chat/completions", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${env.OPENAI_API_KEY}`,
        "Content-Type": "application/json"
      },
      body: bodyText
    });

    const text = await upstream.text();
    return new Response(text, {
      status: upstream.status,
      headers: {
        "Content-Type": "application/json; charset=utf-8"
      }
    });
  } catch (e) {
    return json({ error: "Proxy normalize failed", detail: String(e) }, 500);
  }
}

function isClientAuthorized(request, env) {
  const required = (env.CLIENT_BEARER_TOKEN || "").trim();
  if (!required) {
    return true;
  }

  const auth = request.headers.get("Authorization") || "";
  if (!auth.toLowerCase().startsWith("bearer ")) {
    return false;
  }

  const token = auth.slice(7).trim();
  return token === required;
}

function withCors(response, env) {
  const headers = new Headers(response.headers);
  headers.set("Access-Control-Allow-Origin", (env.ALLOWED_ORIGIN || "*").trim() || "*");
  headers.set("Access-Control-Allow-Headers", "Authorization, Content-Type");
  headers.set("Access-Control-Allow-Methods", "POST, OPTIONS");
  headers.set("Vary", "Origin");

  return new Response(response.body, {
    status: response.status,
    headers
  });
}

function json(body, status = 200) {
  return new Response(JSON.stringify(body), {
    status,
    headers: {
      "Content-Type": "application/json; charset=utf-8"
    }
  });
}
