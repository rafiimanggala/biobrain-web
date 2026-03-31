import { Injectable } from "@angular/core";
import moment from "moment";

@Injectable({
    providedIn: 'root',
  })
export class Logger{
    private _log: string[] = [];

    public log(message: string){
        this._log.push(`${moment.utc().format("DD/MM/YYYY hh:mm:ss")} - INFO - ${message}`);
    }

    public logError(message: string){
        this._log.push(`${moment.utc().format("DD/MM/YYYY hh:mm:ss")} - ERROR - ${message}`);
    }

    public getLog(): string[] { return this._log;}
}