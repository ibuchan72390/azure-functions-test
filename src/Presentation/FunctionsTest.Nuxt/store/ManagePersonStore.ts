import { IManagePersonState } from '~/models/IManagePersonState'
import { GetterTree, MutationTree, ActionTree, ActionContext } from 'vuex'
import { ManagePersonStoreKeys } from '~/models/ManagePersonStoreKeys'
import { ApiSourceStoreKeys, PersonListStoreKeys, IPerson } from '~/models'

export const state = (): IManagePersonState => {
  return {
    id: null,
    name: '',
    submitting: false
  }
}

export const getters: GetterTree<IManagePersonState, {}> = {
  [ManagePersonStoreKeys.getters.isEdit](STATE: IManagePersonState): boolean {
    return STATE.id !== null
  },

  [ManagePersonStoreKeys.getters.submitDisabled](
    STATE: IManagePersonState
  ): boolean {
    return STATE.name.length === 0
  }
}

export const mutations: MutationTree<IManagePersonState> = {
  [ManagePersonStoreKeys.mutations.setId](
    STATE: IManagePersonState,
    id: string | null
  ): void {
    STATE.id = id
  },

  [ManagePersonStoreKeys.mutations.setName](
    STATE: IManagePersonState,
    name: string
  ): void {
    STATE.name = name
  },

  [ManagePersonStoreKeys.mutations.setPerson](
    STATE: IManagePersonState,
    person: IPerson
  ): void {
    STATE.id = person.id
    STATE.name = person.name
  },

  [ManagePersonStoreKeys.mutations.setSubmitting](
    STATE: IManagePersonState,
    submitting: boolean
  ): void {
    STATE.submitting = submitting
  }
}

export const actions: ActionTree<IManagePersonState, {}> = {
  async [ManagePersonStoreKeys.actions.initializeById](
    context: ActionContext<IManagePersonState, {}>,
    id: string
  ): Promise<void> {
    const urlBase =
      context.rootGetters[
        ApiSourceStoreKeys.namespace +
          '/' +
          ApiSourceStoreKeys.getters.getApiLink
      ]

    const url = urlBase + '/api/Presentation-Person-Get?id=' + id

    const response = await fetch(url)

    const personResponse: { person: IPerson } = await response.json()

    context.commit(
      ManagePersonStoreKeys.mutations.setPerson,
      personResponse.person
    )
  },

  async [ManagePersonStoreKeys.actions.submit](
    context: ActionContext<IManagePersonState, {}>
  ): Promise<void> {
    const urlBase =
      context.rootGetters[
        ApiSourceStoreKeys.namespace +
          '/' +
          ApiSourceStoreKeys.getters.getApiLink
      ]

    const isEdit = context.getters[ManagePersonStoreKeys.getters.isEdit]

    let linkSegment: string
    let model: any

    if (!isEdit) {
      model = {
        Name: context.state.name
      }

      linkSegment = '/api/Presentation-Person-Create'
    } else {
      model = {
        Id: context.state.id,
        Name: context.state.name
      }

      linkSegment = '/api/Presentation-Person-Update'
    }

    const url = 'http://' + urlBase + linkSegment

    context.commit(ManagePersonStoreKeys.mutations.setSubmitting, true)

    await fetch(url, { method: 'POST', body: JSON.stringify(model) })

    context.dispatch(
      PersonListStoreKeys.namespace +
        '/' +
        PersonListStoreKeys.actions.initialize,
      null,
      { root: true }
    )

    context.commit(ManagePersonStoreKeys.mutations.setSubmitting, false)
  }
}
