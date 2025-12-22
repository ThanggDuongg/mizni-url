import http from "k6/http";

export interface PrepareResult {
  codes: string[];
}

interface CreateUrlEntryResponse {
  id: string;
  code: string;
  originalUrl: string;
  expires?: string | null;
}

export function prepareData(baseUrl: string): PrepareResult {
  const codes: string[] = [];

  for (let i = 0; i < 20; i++) {
    const res = http.post(
      `${baseUrl}/v1/api/url-entry`,
      JSON.stringify({
        originalUrl: `https://example.com/${i}`,
        expires: null,
      }),
      {
        headers: { "Content-Type": "application/json" },
      }
    );

    console.log(`[prepare] status=${res.status} body=${res.body}`);

    if (res.status !== 200) {
      console.error("[prepare] failed request");
      continue;
    }

    const json = res.json() as any;

    const body: CreateUrlEntryResponse = {
      id: String(json.id),
      code: String(json.code),
      originalUrl: String(json.originalUrl),
      expires: json.expires ?? null,
    };

    if (!body?.code) {
      console.error("[prepare] missing code", res.body);
      continue;
    }

    codes.push(body.code);
  }

  console.log(`[prepare] prepared ${codes.length} codes`);
  return { codes };
}
