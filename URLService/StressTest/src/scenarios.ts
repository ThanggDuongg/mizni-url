import { Options } from "k6/options";

export const scenarios: Options["scenarios"] = {
  steady: {
    executor: "constant-arrival-rate",
    rate: 12,
    timeUnit: "1s",
    duration: "1m",
    preAllocatedVUs: 15,
    maxVUs: 30,
  },

  burst: {
    executor: "constant-arrival-rate",
    rate: 40,
    timeUnit: "1s",
    duration: "5s",
    preAllocatedVUs: 50,
    maxVUs: 100,
  },

  spike: {
    executor: "ramping-arrival-rate",
    startRate: 10,
    timeUnit: "1s",
    stages: [
      { duration: "10s", target: 15 },
      { duration: "5s", target: 80 },
      { duration: "8s", target: 80 },
      { duration: "10s", target: 15 },
      { duration: "5s", target: 0 },
    ],
    preAllocatedVUs: 100,
    maxVUs: 150,
  },

  soak: {
    executor: "constant-arrival-rate",
    rate: 10,
    timeUnit: "1s",
    duration: "10m",
    preAllocatedVUs: 15,
    maxVUs: 30,
  },
};
