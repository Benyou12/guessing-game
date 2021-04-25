export interface IBase {
    toObject?: Function
}

export class Base implements Base {
    public toObject(): object {
        return Object.assign({}, this);
    }
}