import { Options } from 'k6/options';
import { prepareData, PrepareResult } from './prepare';
import { redirectAndRateLimitTest } from './redirectRate';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5000';

export const options: Options = {
  vus: 50,
  duration: '30s',
  thresholds: {
    rate_limited_requests: ['count>0'],
    http_req_failed: ['rate<0.8'],
  },
};

export function setup(): PrepareResult {
  return prepareData(BASE_URL);
}

export default function (data: PrepareResult) {
  redirectAndRateLimitTest(BASE_URL, data.codes);
}
