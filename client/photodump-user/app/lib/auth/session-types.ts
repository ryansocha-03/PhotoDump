export enum SessionTypes {
    Anonymous,
    Guest
}

export interface SessionTypeModel {
    sessionType: SessionTypes
}