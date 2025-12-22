export function fakeIp(vu: number): string {
  return `10.0.${Math.floor(vu / 255)}.${vu % 255}`;
}
