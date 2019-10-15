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
    clear: 'CLEAR',
    setId: 'SET_ID',
    setLoading: 'SET_LOADING',
    setName: 'SET_NAME',
    setPerson: 'SET_PERSON',
    setSubmitting: 'SET_SUBMITTING'
  },
  namespace: 'ManagePersonStore',
  state: {
    id: 'id',
    loading: 'loading',
    name: 'name',
    submitting: 'submitting'
  }
}
