export interface IApiSourceState {
  apiSources: IApiSource[]
  activeApiSource: IApiSource
}

export interface IApiSource {
  name: string
  url: string
}
