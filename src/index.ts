import "bootstrap/dist/css/bootstrap.css"
import "./css/main.css";
import * as signalR from "@microsoft/signalr";
import Cpu from "./models/Cpu";

const htmlElement: HTMLHtmlElement = document.querySelector("html");
const aElement: HTMLElement = document.querySelector("#a-data");
const bElement: HTMLElement = document.querySelector("#b-data");
const cElement: HTMLElement = document.querySelector("#c-data");
const dElement: HTMLElement = document.querySelector("#d-data");
const eElement: HTMLElement = document.querySelector("#e-data");
const hElement: HTMLElement = document.querySelector("#h-data");
const lElement: HTMLElement = document.querySelector("#l-data");
const signElement: HTMLElement = document.querySelector("#sign-data");
const zeroElement: HTMLElement = document.querySelector("#zero-data");
const auxCarryElement: HTMLElement = document.querySelector("#auxcarry-data");
const parityElement: HTMLElement = document.querySelector("#parity-data");
const carryElement: HTMLElement = document.querySelector("#carry-data");
const stackPointerElement: HTMLElement = document.querySelector("#stack-pointer-data");
const programCounterElement: HTMLElement = document.querySelector("#program-counter-data");
const interruptsEnabledElement: HTMLElement = document.querySelector("#interrupts-enabled-data");
const cyclesPerSecondElement: HTMLElement = document.querySelector("#cycles-per-second");
const uiCanvas: HTMLCanvasElement = document.querySelector("#ui-canvas");
const backingCanvas: HTMLCanvasElement = document.querySelector("#backing-canvas");
const canvasContext = uiCanvas.getContext("2d");
const backingCanvasContext = backingCanvas.getContext("2d");

let memoryElements: HTMLElement[] = [];
for (let ii = 0; ii < 0x100; ii++) {
    memoryElements.push(document.querySelector(`#memory-value-${(0x2000 + ii).toString(16).toUpperCase()}`));
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalr/hub")
    .withAutomaticReconnect()
    .build();

let timeSinceLastUpdate: Date = null;
let cyclesAtLastUpdate: number = 0;
connection.on("cpuUpdated", (cpu: Cpu, memoryBase64String: string) => {
    aElement.textContent = `0x${cpu.state.a.toString(16).toUpperCase()}`;
    bElement.textContent = `0x${cpu.state.b.toString(16).toUpperCase()}`;
    cElement.textContent = `0x${cpu.state.c.toString(16).toUpperCase()}`;
    dElement.textContent = `0x${cpu.state.d.toString(16).toUpperCase()}`;
    eElement.textContent = `0x${cpu.state.e.toString(16).toUpperCase()}`;
    hElement.textContent = `0x${cpu.state.h.toString(16).toUpperCase()}`;
    lElement.textContent = `0x${cpu.state.l.toString(16).toUpperCase()}`;
    signElement.textContent = `${cpu.state.flags.sign ? "True" : "False"}`;
    zeroElement.textContent = `${cpu.state.flags.zero ? "True" : "False"}`;
    auxCarryElement.textContent = `${cpu.state.flags.auxCarry ? "True" : "False"}`;
    parityElement.textContent = `${cpu.state.flags.parity ? "True" : "False"}`;
    carryElement.textContent = `${cpu.state.flags.carry ? "True" : "False"}`;
    stackPointerElement.textContent = `0x${cpu.state.stackPointer.toString(16).toUpperCase()}`;
    programCounterElement.textContent = `0x${cpu.state.programCounter.toString(16).toUpperCase()}`;
    interruptsEnabledElement.textContent = `${cpu.state.interruptsEnabled ? "True" : "False"}`;

    const memoryArray = atob(memoryBase64String);
    for (let ii = 0; ii < memoryArray.length; ii++) {
        memoryElements[ii].textContent = `0x${memoryArray.charCodeAt(ii).toString(16).toUpperCase()}`;
    }

    const updateTime = new Date();
    if (timeSinceLastUpdate !== null) {
        const diffMs = updateTime.getTime() - timeSinceLastUpdate.getTime();
        const diffCycles = cpu.state.cycles - cyclesAtLastUpdate;

        // If cycles haven't changed don't change the performance counter
        if (diffCycles !== 0) {
            const cyclesPerSecond = (diffCycles * 1000) / diffMs;
            cyclesPerSecondElement.textContent = `${cyclesPerSecond.toFixed(2)}Hz`;
            timeSinceLastUpdate = updateTime;
            cyclesAtLastUpdate = cpu.state.cycles;
        }
    } else {
        timeSinceLastUpdate = updateTime;
        cyclesAtLastUpdate = cpu.state.cycles;
    }    
});

connection.on("vblank", (vram: string) => {
    const vramBytes = atob(vram);
    const arr = new Uint8ClampedArray(224 * 256 * 4);

    for (let arrIx = 0; arrIx < arr.length; arrIx += 4) {
        const x = (arrIx / 4) % 224;
        const y = Math.floor((arrIx / 4) / 256);
        const rX = 255 - y;
        const rY = x;
        const vramIx = Math.floor((rY * 256 + rX) / 8);
        const pixel = rX % 8;
        const value = ((vramBytes.charCodeAt(vramIx) >> pixel) & 1) == 1 ? 0xFF : 0x0;

        arr[arrIx] = value; // R
        arr[arrIx + 1] = value; // G
        arr[arrIx + 2] = value; // B
        arr[arrIx + 3] = 0xFF; // A
    }
    
    const imageData = new ImageData(arr, 224);    
    backingCanvasContext.putImageData(imageData, 0, 0);
    canvasContext.drawImage(backingCanvas, 0, 0, 224, 256, 0, 0, 448, 512);
});

connection.start().catch(err => document.write(err));

htmlElement.addEventListener("keyup", (e: KeyboardEvent) => {
    connection.send("keyUp", e.key);
});

htmlElement.addEventListener("keydown", (e: KeyboardEvent) => {
    connection.send("keyDown", e.key);
});