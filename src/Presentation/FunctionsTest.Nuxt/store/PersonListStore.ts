import { GetterTree, ActionTree, ActionContext, MutationTree } from 'vuex'
import { IPersonListState } from '~/models/IPersonListState'
import { IPerson } from '~/models/IPerson'
import { PersonListStoreKeys } from '~/models/PersonListStoreKeys'
import { ApiSourceStoreKeys } from '~/models'

export const state = (): IPersonListState => {
  return {
    people: null
  }
}

export const getters: GetterTree<IPersonListState, {}> = {
  [PersonListStoreKeys.getters.isLoading](STATE: IPersonListState): boolean {
    return STATE.people === null
  }
}

export const mutations: MutationTree<IPersonListState> = {
  [PersonListStoreKeys.mutations.setPeople](
    STATE: IPersonListState,
    people: IPerson[]
  ): void {
    STATE.people = people
  }
}

export const actions: ActionTree<IPersonListState, {}> = {
  async [PersonListStoreKeys.actions.initialize](
    context: ActionContext<IPersonListState, {}>
  ): Promise<void> {
    // configure the loading state
    context.commit(PersonListStoreKeys.mutations.setPeople, null)

    const url =
      context.rootGetters[
        ApiSourceStoreKeys.namespace +
          '/' +
          ApiSourceStoreKeys.getters.getApiLink
      ]

    const reqInfo: RequestInfo =
      'http://' + url + '/api/Presentation-Person-GetAll'

    const fetchResult = await fetch(reqInfo)

    const people: IPerson = ((await fetchResult.json()) as any).people

    context.commit(PersonListStoreKeys.mutations.setPeople, people)
  }
}
