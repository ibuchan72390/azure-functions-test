import { GetterTree, MutationTree } from 'vuex/types/index'
import { IApiSource, IApiSourceState } from '~/models/IApiSourceState'
import { ApiSourceStoreKeys } from '~/models/ApiSourceStoreKeys'

function linkGen(): IApiSource[] {
  return [
    { name: 'Visual Studio', url: 'localhost:7071' },
    // docker performance SUCKSSSSS on my home computer, but that is only docker toolbox

    { name: 'Docker', url: '192.168.99.100:7071' }
    // Need to deploy and get a live URL to see performance over there
    // Then we can switch to a service-bus style technology to see the performance differences
  ]
}

export const state = (): IApiSourceState => {
  const links = linkGen()

  return {
    activeApiSource: links[0],
    apiSources: links
  }
}

export const getters: GetterTree<IApiSourceState, {}> = {
  [ApiSourceStoreKeys.getters.getApiLink](STATE: IApiSourceState): string {
    return STATE.activeApiSource.url
  }
}

export const mutations: MutationTree<IApiSourceState> = {
  [ApiSourceStoreKeys.mutations.setApiSource](
    STATE: IApiSourceState,
    source: IApiSource
  ): void {
    STATE.activeApiSource = source
  }
}
