import {
  combine,
  createEffect,
  createEvent,
  createStore,
  sample,
} from "effector";
import { NavigateFunction } from "react-router-dom";

const setNavigator = createEvent<NavigateFunction>();
const setAppLoading = createEvent<boolean>();
const setHeaderTitle = createEvent<string>();
const goTo = createEvent<string>();

const goToFx = createEffect(
  (props: { nav: NavigateFunction; path: string }) => {
    props.nav(props.path);
  }
);

const $appTitle = createStore<string>("").on(setHeaderTitle, (_, e) => e);
const $appLoading = createStore<boolean>(false).on(setAppLoading, (_, e) => e);
const $navigator = createStore<NavigateFunction | null>(null).on(
  setNavigator,
  (_, e) => e
);

sample({
  clock: goTo,
  source: { nav: $navigator },
  filter: ({ nav }) => !!nav,
  fn: ({ nav }, path) => ({ nav: nav as NavigateFunction, path }),
  target: goToFx,
});

const $app = combine({
  appLoading: $appLoading,
  title: $appTitle,
  navigator: $navigator,
});

export { $app, setHeaderTitle, setNavigator, goTo };
