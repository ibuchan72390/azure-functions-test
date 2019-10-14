export const ManagePersonStoreKeys = {
  actions: {
    delete: 'delete',
    initializeById: 'initalizeById',
    submit: 'submit'
  },
  getters: {
    isEdit: 'isEdit',
    submitDisabled: 'submitDisabled'
  },
  mutations: {
    setId: 'SET_ID',
    setName: 'SET_NAME',
    setPerson: 'SET_PERSON',
    setSubmitting: 'SET_SUBMITTING'
  },
  namespace: 'ManagePersonStore',
  state: {
    id: 'id',
    name: 'name',
    submitting: 'submitting'
  }
}
