export default class CpuFlags {
    sign: boolean;
    zero: boolean;
    auxCarry: boolean;
    parity: boolean;
    carry: boolean;

    constructor(sign: boolean, zero: boolean, auxCarry: boolean, parity: boolean, carry: boolean) {
        this.sign = sign;
        this.zero = zero;
        this.auxCarry = auxCarry;
        this.parity = parity;
        this.carry = carry;
    }
}
