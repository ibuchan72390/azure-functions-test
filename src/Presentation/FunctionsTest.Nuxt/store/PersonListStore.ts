import { IPersonListState } from '~/models/IPersonListState'
import { GetterTree, ActionTree, ActionContext, MutationTree } from 'vuex'
import { IPerson } from '~/models/IPerson'
import { PersonListStoreKeys } from '~/models/PersonListStoreKeys'

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
    const reqInfo: RequestInfo =
      'http://localhost:7071/api/Presentation-Person-GetAll'

    const fetchResult = await fetch(reqInfo)

    const people: IPerson = await fetchResult.json()

    context.commit(PersonListStoreKeys.mutations.setPeople, people)
  }
}
