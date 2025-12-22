import http, { Params } from "k6/http";
import { check, sleep } from "k6";
import { Counter } from "k6/metrics";
import { fakeIp } from "./utils";

export const redirectedRequests = new Counter("redirected_requests");
export const rateLimitedRequests = new Counter("rate_limited_requests");

export function redirectAndRateLimitTest(
  baseUrl: string,
  codes: string[],
  multiIp = false
) {
  const code = codes[Math.floor(Math.random() * codes.length)];

  const headers: Record<string, string> = {};
  if (multiIp) {
    headers["X-Forwarded-For"] = fakeIp(__VU);
  }

  const params: Params = {
    redirects: 0,
    headers,
  };

  const res = http.get(`${baseUrl}/${code}`, params);

  check(res, {
    "status is valid (302 or 429)": (r) => r.status === 302 || r.status === 429,
  });

  if (res.status === 302) {
    redirectedRequests.add(1);
  }

  if (res.status === 429) {
    rateLimitedRequests.add(1);
  }

  sleep(0.02);
}
