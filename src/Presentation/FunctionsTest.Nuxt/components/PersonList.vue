<template>
  <div>
    <h2>Person List</h2>
    <button @click="init">Refresh</button>
    <button @click="add">Add</button>
    <div v-if="isLoading">Loading...</div>
    <div v-if="!isLoading">
      <div
        v-for="person of people"
        :key="person.id"
        style="border: 1px solid gray; margin: 12px; text-align: left; width: 300px; display: flex; justify-content: space-between;"
      >
        <div>
          <div>Name: {{person.name}}</div>
          <div>Id: {{person.id}}</div>
        </div>

        <button @click="edit(person)">Edit</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component, namespace } from 'nuxt-property-decorator'

import { PersonListStoreKeys } from '@/models/PersonListStoreKeys'
import { IPerson } from '@/models/IPerson'
import { ManagePersonStoreKeys } from '../models/ManagePersonStoreKeys'

const personListStore = namespace(PersonListStoreKeys.namespace)
const personStore = namespace(ManagePersonStoreKeys.namespace)

@Component
export default class PersonList extends Vue {
  /**
   * Refresh the page, see how there is no data flash
   * Comment this out and refresh the page... notice the data flash
   * That is SSR in action in which we are hydrating the page to render server side
   */
  public async serverPrefetch() {
    await this.init()
  }

  public async mounted() {
    /**
     * Expressly check if still isLoading in mounted, this is the first client-only hook
     * If the above serverPrefetch has not completed, this will need to be completed
     *
     * isLoading will tell us if serverPrefetch was fired because the data loaded on the
     * server into the store will flow to the client seamlessly.
     *
     * If you comment this out, then go to /create, then create a person, the page will never load.
     */
    if (this.isLoading) {
      await this.init()
    }
  }

  public add() {
    this.$router.push('/manage')
  }

  public edit(person: IPerson) {
    this.$router.push('/manage/' + person.id)
  }

  @personListStore.Getter(PersonListStoreKeys.getters.isLoading)
  public isLoading: boolean

  @personListStore.State(PersonListStoreKeys.state.people)
  public people: IPerson[]

  @personListStore.Action(PersonListStoreKeys.actions.initialize)
  public init: () => Promise<void>

  @personStore.Mutation(ManagePersonStoreKeys.mutations.setPerson)
  public setPerson: (person: IPerson) => void
}
</script>
