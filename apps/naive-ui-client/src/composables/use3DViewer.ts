import { ref, watch } from 'vue';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

export interface Viewer3DState {
  scene: THREE.Scene;
  camera: THREE.PerspectiveCamera;
  renderer: THREE.WebGLRenderer;
  controls: OrbitControls;
  gridHelper: THREE.GridHelper;
  axesHelper: THREE.AxesHelper;
  raycaster: THREE.Raycaster;
  mouse: THREE.Vector2;
}

export interface Viewer3DOptions {
  showGrid?: boolean;
  showAxes?: boolean;
  containerElement: HTMLElement;
  canvasElement?: HTMLCanvasElement;
  onAnimate?: (state: Viewer3DState) => void;
  sceneBgColor?: number;
}

export function use3DViewer(options: Viewer3DOptions) {
  const showGrid = ref(options.showGrid ?? true);
  const showAxes = ref(options.showAxes ?? true);
  let state: Viewer3DState | null = null;
  let animationId: number | null = null;
  let resizeObserver: ResizeObserver | null = null;
  let resizeTimeout: number | null = null;

  function initScene(): Viewer3DState {
    const container = options.containerElement;
    if (!container) {
      throw new Error('Container element is required');
    }

    const width = container.clientWidth;
    const height = container.clientHeight;

    // Scene
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(options.sceneBgColor ?? 0x1a1a1a);

    // Camera
    const camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 10000);
    camera.position.set(400, 300, 400);

    // Renderer
    const renderer = new THREE.WebGLRenderer({
      canvas: options.canvasElement,
      antialias: true
    });
    renderer.setSize(width, height);
    renderer.setPixelRatio(window.devicePixelRatio);

    if (!options.canvasElement) {
      container.appendChild(renderer.domElement);
    }

    // Controls
    const controls = new OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;
    controls.dampingFactor = 0.05;

    // Lights
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
    scene.add(ambientLight);

    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(100, 200, 100);
    scene.add(directionalLight);

    // Grid
    const gridHelper = new THREE.GridHelper(1000, 20);
    gridHelper.visible = showGrid.value;
    scene.add(gridHelper);

    // Axes
    const axesHelper = new THREE.AxesHelper(150);
    axesHelper.visible = showAxes.value;
    scene.add(axesHelper);

    // Raycaster for hover detection
    const raycaster = new THREE.Raycaster();
    const mouse = new THREE.Vector2();

    return {
      scene,
      camera,
      renderer,
      controls,
      gridHelper,
      axesHelper,
      raycaster,
      mouse
    };
  }

  function startAnimation() {
    if (animationId !== null) {
      return;
    }

    function animate() {
      animationId = requestAnimationFrame(animate);

      if (state) {
        state.controls.update();
        options.onAnimate?.(state);
        state.renderer.render(state.scene, state.camera);
      }
    }

    animate();
  }

  function stopAnimation() {
    if (animationId !== null) {
      cancelAnimationFrame(animationId);
      animationId = null;
    }
  }

  function onWindowResize() {
    if (!state || !options.containerElement) return;

    const width = options.containerElement.clientWidth;
    const height = options.containerElement.clientHeight;

    state.camera.aspect = width / height;
    state.camera.updateProjectionMatrix();
    state.renderer.setSize(width, height);
  }

  function setupResizeObserver() {
    if (!options.containerElement) return;

    resizeObserver = new ResizeObserver(() => {
      if (resizeTimeout !== null) {
        clearTimeout(resizeTimeout);
      }
      resizeTimeout = window.setTimeout(() => {
        onWindowResize();
      }, 100);
    });

    resizeObserver.observe(options.containerElement);
  }

  function fitCameraToContent() {
    if (!state) return;

    const box = new THREE.Box3();

    state.scene.traverse((object) => {
      if (
        object instanceof THREE.Mesh &&
        !(object instanceof THREE.GridHelper) &&
        !(object instanceof THREE.AxesHelper)
      ) {
        box.expandByObject(object);
      }
    });

    if (box.isEmpty()) {
      state.camera.position.set(400, 300, 400);
      state.controls.target.set(0, 0, 0);
    } else {
      const center = box.getCenter(new THREE.Vector3());
      const size = box.getSize(new THREE.Vector3());
      const maxDim = Math.max(size.x, size.y, size.z);
      const fov = state.camera.fov * (Math.PI / 180);
      let cameraZ = Math.abs((maxDim / 2) / Math.tan(fov / 2));
      cameraZ *= 1.5;

      state.camera.position.set(
        center.x + cameraZ * 0.7,
        center.y + cameraZ * 0.5,
        center.z + cameraZ * 0.7
      );
      state.controls.target.copy(center);
    }

    state.controls.update();
  }

  function mount() {
    state = initScene();
    startAnimation();
    setupResizeObserver();

    window.addEventListener('resize', onWindowResize);

    // Watch grid and axes visibility
    watch(showGrid, (value) => {
      if (state) {
        state.gridHelper.visible = value;
      }
    });

    watch(showAxes, (value) => {
      if (state) {
        state.axesHelper.visible = value;
      }
    });

    return state;
  }

  function unmount() {
    window.removeEventListener('resize', onWindowResize);

    if (resizeTimeout !== null) {
      clearTimeout(resizeTimeout);
    }

    if (resizeObserver) {
      resizeObserver.disconnect();
    }

    stopAnimation();

    if (state) {
      state.renderer.dispose();
      state.scene.traverse((object) => {
        if (object instanceof THREE.Mesh) {
          object.geometry.dispose();
          if (Array.isArray(object.material)) {
            object.material.forEach((m) => m.dispose());
          } else {
            object.material.dispose();
          }
        }
      });
    }
  }

  function highlightCollisions(
    meshMap: Map<string, THREE.Mesh>,
    collisionIds: string[],
    getColorForObject: (mesh: THREE.Mesh) => number
  ) {
    meshMap.forEach((mesh) => {
      const isColliding = collisionIds.includes(mesh.userData?.id || '');
      const color = isColliding ? 0xff0000 : getColorForObject(mesh);

      if (Array.isArray(mesh.material)) {
        mesh.material.forEach((m) => {
          if (m instanceof THREE.MeshStandardMaterial) {
            m.color.set(color);
          }
        });
      } else if (mesh.material instanceof THREE.MeshStandardMaterial) {
        mesh.material.color.set(color);
      }
    });
  }

  return {
    showGrid,
    showAxes,
    getState: () => state,
    mount,
    unmount,
    onWindowResize,
    fitCameraToContent,
    resetCamera: () => fitCameraToContent(),
    highlightCollisions
  };
}
