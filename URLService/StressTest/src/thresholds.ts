import { Options } from "k6/options";

export const thresholds: Record<string, Options["thresholds"]> = {
  steady: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<500"],
    rate_limited_requests: ["count==0"],
  },

  burst: {
    http_req_failed: ["rate<0.5"],
    http_req_duration: ["p(95)<1000"],
    rate_limited_requests: ["count>0", "count<150"],
  },

  spike: {
    http_req_failed: ["rate>0.5"],
    http_req_duration: ["p(95)<3000"],
    rate_limited_requests: ["count>400"],
  },

  soak: {
    http_req_failed: ["rate<0.02"],
    http_req_duration: ["p(95)<800"],
    rate_limited_requests: ["count<10"],
  },
};
