import CpuState from "./CpuState";

export default class Cpu {
    opcode: number;
    state: CpuState;
    id: string;

    constructor(id: string, state: CpuState, opcode: number) {
        this.id = id;
        this.state = state;
        this.opcode = opcode;
    }
}
