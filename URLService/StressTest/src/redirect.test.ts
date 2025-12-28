import { Options } from "k6/options";
import { prepareData, PrepareResult } from "./prepare";
import { redirectAndRateLimitTest } from "./redirectRate";
import { scenarios } from "./scenarios";
import { thresholds } from "./thresholds";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5000";
const MODE = __ENV.MODE || "steady"; // steady | burst | spike | soak
const MULTI_IP = __ENV.MULTI_IP === "true";
const FAKE_CODES = __ENV.FAKE_CODES === "true";

export const options: Options = {
  scenarios: {
    [MODE]: scenarios![MODE],
  },

  thresholds: thresholds[MODE],
};

export function setup(): PrepareResult {
  return prepareData(BASE_URL);
}

export default function (data: PrepareResult) {
  const fake_codes = ["aaa", "bbb", "ccc", "ddd", "eee", "aaa", "bbb","bbb","bbb","bbb","ccc", "aaa"];
  const codes = FAKE_CODES ? [...data.codes, ...fake_codes] : data.codes;
  redirectAndRateLimitTest(BASE_URL, codes, MULTI_IP);
}
