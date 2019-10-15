<template>
  <div>
    <div v-if="isLoading">Loading...</div>
    <div v-if="!isLoading">
      <div v-if="isEdit">
        <label for="id">Id</label>
        <input type="text" id="id" name="id" disabled="disabled" :value="id" />
      </div>

      <label for="name">Name</label>
      <input type="text" id="name" name="name" v-model="name" />

      <div v-if="!submitting">
        <button :disabled="disabled" @click="submit">Submit</button>
        <button v-if="isEdit" @click="deleteEntity()">Delete</button>
      </div>
      <div v-if="submitting">Submitting...</div>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, namespace, Prop } from 'nuxt-property-decorator'
import { ManagePersonStoreKeys } from '@/models/ManagePersonStoreKeys'

const managePerson = namespace(ManagePersonStoreKeys.namespace)

@Component
export default class ManagePerson extends Vue {
  public async mounted() {
    await this.initialize()
  }

  public destroyed() {
    this.clear()
  }

  public async serverPrefetch() {
    await this.initialize()
  }

  public async submit() {
    await this.internalSubmit()
    this.backToList()
  }

  public async deleteEntity() {
    await this.internalDelete()
    this.backToList()
  }

  public get name() {
    return this.storeName
  }
  public set name(val) {
    this.setName(val)
  }

  private initialize() {
    if (this.initId && !this.id) {
      this.initializeById(this.initId)
    }
  }

  private backToList(): void {
    this.$router.push('/')
  }

  @managePerson.State(ManagePersonStoreKeys.state.loading)
  public isLoading: boolean

  @managePerson.State(ManagePersonStoreKeys.state.id)
  public id: string

  @managePerson.State(ManagePersonStoreKeys.state.name)
  public storeName: string

  @managePerson.State(ManagePersonStoreKeys.state.submitting)
  public submitting: boolean

  @managePerson.Getter(ManagePersonStoreKeys.getters.isEdit)
  public isEdit: boolean

  @managePerson.Getter(ManagePersonStoreKeys.getters.submitDisabled)
  public disabled: boolean

  @managePerson.Mutation(ManagePersonStoreKeys.mutations.clear)
  private clear: () => void

  @managePerson.Mutation(ManagePersonStoreKeys.mutations.setName)
  private setName: (name: string) => void

  @managePerson.Action(ManagePersonStoreKeys.actions.submit)
  private internalSubmit: () => Promise<void>

  @managePerson.Action(ManagePersonStoreKeys.actions.initializeById)
  private initializeById: (id: string) => Promise<void>

  @managePerson.Action(ManagePersonStoreKeys.actions.delete)
  private internalDelete: () => Promise<void>

  @Prop()
  private initId: string
}
</script>
