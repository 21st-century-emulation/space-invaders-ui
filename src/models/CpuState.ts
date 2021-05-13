import CpuFlags from "./CpuFlags";

export default class CpuState {
    a: number;
    b: number;
    c: number;
    d: number;
    e: number;
    h: number;
    l: number;
    stackPointer: number;
    programCounter: number;
    cycles: number;
    interruptsEnabled: boolean;
    flags: CpuFlags;

    constructor(a: number, b: number, c: number, d: number, e: number, h: number, l: number, stackPointer: number, programCounter: number, cycles: number, interruptsEnabled: boolean, flags: CpuFlags) {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.h = h;
        this.l = l;
        this.stackPointer = stackPointer;
        this.programCounter = programCounter;
        this.cycles = cycles;
        this.interruptsEnabled = interruptsEnabled;
        this.flags = flags;
    }
}
