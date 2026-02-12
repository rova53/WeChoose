export enum DeliveryMode {
    Remote,
    InPerson
}

export const deliveryModeLabels: Record<DeliveryMode, string> = {
    [DeliveryMode.Remote]: "Distanciel",
    [DeliveryMode.InPerson]: "Présentiel"
};