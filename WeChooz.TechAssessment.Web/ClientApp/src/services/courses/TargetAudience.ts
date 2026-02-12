export enum TargetAudience {
    CseElected,
    CsePresident
}

export const targetAudienceLabels: Record<TargetAudience, string> = {
    [TargetAudience.CseElected]: "Élu CSE",
    [TargetAudience.CsePresident]: "Président CSE"
};